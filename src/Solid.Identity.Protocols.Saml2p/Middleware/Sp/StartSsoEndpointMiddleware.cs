using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Solid.Identity.Protocols.Saml2p.Abstractions;
using Solid.Identity.Protocols.Saml2p.Areas.__Saml2p.Pages;
using Solid.Identity.Protocols.Saml2p.Cache;
using Solid.Identity.Protocols.Saml2p.Factories;
using Solid.Identity.Protocols.Saml2p.Models;
using Solid.Identity.Protocols.Saml2p.Models.Context;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using Solid.Identity.Protocols.Saml2p.Options;
using Solid.Identity.Protocols.Saml2p.Providers;
using Solid.Identity.Protocols.Saml2p.Serialization;
using Solid.Identity.Protocols.Saml2p.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.Saml2p.Middleware.Sp
{
    internal class StartSsoEndpointMiddleware : Saml2pEndpointMiddleware
    {
        private RazorPageRenderingService _razor;
        private AuthnRequestFactory _authnRequestFactory;

        public StartSsoEndpointMiddleware(RazorPageRenderingService razor, AuthnRequestFactory authnRequestFactory, Saml2pSerializer serializer, Saml2pCache cache, Saml2pPartnerProvider partners, IOptionsMonitor<Saml2pOptions> monitor, ILoggerFactory factory, RequestDelegate _)
            : this(razor, authnRequestFactory, serializer, cache, partners, monitor, factory)
        {
        }
        public StartSsoEndpointMiddleware(RazorPageRenderingService razor, AuthnRequestFactory authnRequestFactory, Saml2pSerializer serializer, Saml2pCache cache, Saml2pPartnerProvider partners, IOptionsMonitor<Saml2pOptions> monitor, ILoggerFactory factory)
            : base(serializer, cache, partners, monitor, factory)
        {
            _razor = razor;
            _authnRequestFactory = authnRequestFactory;
        }

        public override Task InvokeAsync(HttpContext context)
        {
            var id = context.Request.Query[Options.PartnerIdQueryParameter];
            if (StringValues.IsNullOrEmpty(id))
                throw new InvalidOperationException($"Missing '{Options.PartnerIdQueryParameter}' query parameter.");
            return InvokeAsync(context, id);
        }

        internal async Task InvokeAsync(HttpContext context, string partnerId)
        {
            Logger.LogInformation("Starting SAML2P authentication (SP flow).");
            var partner = await Partners.GetIdentityProviderAsync(partnerId);
            
            if (partner == null)
                throw new SecurityException($"Partner '{partnerId}' not found.");

            if (!partner.Enabled)
                throw new SecurityException($"Partner '{partnerId}' is disabled.");

            if (!partner.AllowsSpInitiatedSso)
                throw new SecurityException($"SP initiated SSO is not allowed for partner '{partnerId}'.");

            var request = await _authnRequestFactory.CreateAuthnRequestAsync(context, partner);
            await Cache.CacheRequestAsync(request.Id, request);
            var ssoContext = new StartSsoContext
            {
                PartnerId = partnerId,
                Partner = partner,
                AuthnRequest = request
            };
            await Events.InvokeAsync(Options, partner, e => e.OnStartSso(context.RequestServices, ssoContext));

            if (!partner.SupportedBindings.Any())
                throw new InvalidOperationException($"Partner '{partner.Id}' has no supported bindings.");

            var binding = partner.SupportedBindings.First();
            var destination = new Uri(partner.BaseUrl, partner.AcceptSsoEndpoint);

            Trace($"Sending SAMLRequest to '{destination}' using {binding} binding.", request);

            await StartSsoAsync(context, request, destination, binding);
        }

        private Task StartSsoAsync(HttpContext context, AuthnRequest request, Uri destination, BindingType binding)
        {
            var base64 = SerializeAuthnRequest(request, binding);
            if (binding == BindingType.Post) return PostAsync(context, base64, destination, request.RelayState);
            if (binding == BindingType.Redirect) return RedirectAsync(context, base64, destination, request.RelayState);

            throw new ArgumentException($"Unsupported binding type: '{binding}'");
        }

        private async Task RedirectAsync(HttpContext context, string base64, Uri destination, string relayState)
        {
            var queryBuilder = new StringBuilder();
            if (string.IsNullOrEmpty(destination.Query))
                queryBuilder.Append("?");
            else
                queryBuilder.Append("&");
            queryBuilder.Append("SAMLRequest=");
            queryBuilder.Append(base64);
            if (!string.IsNullOrEmpty(relayState))
            {
                queryBuilder.Append("&");
                queryBuilder.Append("RelayState=");
                queryBuilder.Append(relayState);
            }

            var url = $"{destination}{queryBuilder.ToString()}";

            context.Response.Redirect(url, false);
        }

        private async Task PostAsync(HttpContext context, string base64, Uri destination, string relayState)
        {
            var model = new AuthnRequestModel
            {
                Destination = destination,
                SamlRequest = base64
            };
            var html = await _razor.RenderPageAsync(model, "AuthnRequest", "__Saml2p");
            context.Response.StatusCode = 200;
            context.Response.ContentType = "text/html";
            var bytes = Encoding.UTF8.GetBytes(html);
            await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
        }
    }
}
