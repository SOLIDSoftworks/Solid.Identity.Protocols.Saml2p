using Microsoft.AspNetCore.Http;
using Solid.Identity.Protocols.Saml2p.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Solid.Identity.Protocols.Saml2p.Cache;
using Solid.Identity.Protocols.Saml2p.Providers;
using Microsoft.IdentityModel.Tokens.Saml2;
using System.Linq;
using System.Security;
using Solid.Identity.Protocols.Saml2p.Abstractions;
using System.Security.Claims;
using Solid.Identity.Protocols.Saml2p.Models.Context;
using Solid.Identity.Protocols.Saml2p.Serialization;
using Solid.Identity.Protocols.Saml2p.Factories;
using Solid.Identity.Protocols.Saml2p.Areas.__Saml2p.Pages;
using Microsoft.Extensions.Primitives;
using Solid.Identity.Protocols.Saml2p.Services;
using Solid.Identity.Protocols.Saml2p.Models;

namespace Solid.Identity.Protocols.Saml2p.Middleware.Idp
{
    internal class CompleteSsoEndpointMiddleware : Saml2pEndpointMiddleware
    {
        private RazorPageRenderingService _razor;
        private Saml2SecurityTokenHandler _handler;
        private ISecurityTokenDescriptorFactory _descriptorFactory;
        private SamlResponseFactory _responseFactory;

        public CompleteSsoEndpointMiddleware(RazorPageRenderingService razor, Saml2SecurityTokenHandler handler, ISecurityTokenDescriptorFactory descriptorFactory, SamlResponseFactory responseFactory, Saml2pSerializer serializer, Saml2pCache cache, Saml2pPartnerProvider partners, IOptionsMonitor<Saml2pOptions> monitor, ILoggerFactory loggerFactory, RequestDelegate _)
            : base(serializer, cache, partners, monitor, loggerFactory)
        {
            _razor = razor;
            _handler = handler;
            _descriptorFactory = descriptorFactory;
            _responseFactory = responseFactory;
        }

        public override async Task InvokeAsync(HttpContext context)
        {
            Logger.LogInformation("Completing SAML2P authentication (IDP flow).");
            var id = context.Request.Query["id"];
            if(StringValues.IsNullOrEmpty(id))
                throw new InvalidOperationException($"Missing 'id' query parameter.");

            var user = context.User;
            var request = await Cache.FetchRequestAsync(id);
            if (request == null)
                throw new SecurityException($"SAMLRequest not found for id: '{id}'");

            Trace("Found cached SAMLRequest.", request);
            var partnerId = request.Issuer;
            var partner = await Partners.GetServiceProviderAsync(partnerId);

            if (partner == null)
                throw new SecurityException($"Partner '{partnerId}' not found.");

            if (!partner.Enabled)
                throw new SecurityException($"Partner '{partnerId}' is disabled.");

            if (user.Identity.IsAuthenticated)
            {
                var descriptor = await _descriptorFactory.CreateSecurityTokenDescriptorAsync(user.Identity as ClaimsIdentity, partner);
                var createSecurityTokenContext = new CreateSecurityTokenContext
                {
                    PartnerId = partner.Id,
                    Partner = partner,
                    TokenDescriptor = descriptor,
                    Handler = _handler
                };

                await Events.InvokeAsync(Options, partner, e => e.OnCreatingSecurityToken(context.RequestServices, createSecurityTokenContext));

                if (createSecurityTokenContext.SecurityToken == null)
                    createSecurityTokenContext.SecurityToken = createSecurityTokenContext.Handler.CreateToken(createSecurityTokenContext.TokenDescriptor) as Saml2SecurityToken;

                await Events.InvokeAsync(Options, partner, e => e.OnCreatedSecurityToken(context.RequestServices, createSecurityTokenContext));

                var response = _responseFactory.Create(partner, authnRequestId: request.Id, relayState: request.RelayState, token: createSecurityTokenContext.SecurityToken);

                var completeSsoContext = new CompleteSsoContext
                {
                    PartnerId = partner.Id,
                    Partner = partner,
                    Request = request,
                    Response = response
                };
                await Events.InvokeAsync(Options, partner, e => e.OnCompleteSso(context.RequestServices, completeSsoContext));

                if (!partner.SupportedBindings.Any())
                    throw new InvalidOperationException($"Partner '{partner.Id}' has no supported bindings.");

                var binding = partner.SupportedBindings.First();
                Trace($"Sending SAMLResponse using {binding} binding.", response);
                await CompleteSsoAsync(context, response, new Uri(partner.BaseUrl, partner.AssertionConsumerServiceEndpoint), binding);
            }
            else
            {
                // 401?
                // maybe another event
            }
        }

        private Task CompleteSsoAsync(HttpContext context, SamlResponse response, Uri destination, BindingType binding)
        {
            var base64 = SerializeSamlResponse(response, binding);
            if (binding == BindingType.Post) return PostAsync(context, base64, destination, response.RelayState);
            if (binding == BindingType.Redirect) return RedirectAsync(context, base64, destination, response.RelayState);

            throw new ArgumentException($"Unsupported binding type: '{binding}'");
        }

        private async Task RedirectAsync(HttpContext context, string base64, Uri destination, string relayState)
        {
            var queryBuilder = new StringBuilder();
            if (string.IsNullOrEmpty(destination.Query))
                queryBuilder.Append("?");
            else
                queryBuilder.Append("&");
            queryBuilder.Append("SAMLResponse=");
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
            var model = new SamlResponseModel
            {
                Recipient = destination,
                SamlResponse = base64,
                RelayState = relayState
            };
            var html = await _razor.RenderPageAsync(model, "SamlResponse", "__Saml2p");
            context.Response.StatusCode = 200;
            context.Response.ContentType = "text/html";
            var bytes = Encoding.UTF8.GetBytes(html);
            await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
        }
    }
}
