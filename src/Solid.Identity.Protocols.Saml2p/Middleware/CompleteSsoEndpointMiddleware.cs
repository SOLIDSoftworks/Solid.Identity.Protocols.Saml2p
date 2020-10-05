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
using Microsoft.IdentityModel.Tokens.Saml2;
using System.Linq;
using System.Security;
using Solid.Identity.Protocols.Saml2p.Abstractions;
using System.Security.Claims;
using Solid.Identity.Protocols.Saml2p.Models.Context;
using Solid.Identity.Protocols.Saml2p.Serialization;
using Solid.Identity.Protocols.Saml2p.Factories;
using Solid.Identity.Protocols.Saml2p.Areas.__Saml2p.Pages;

namespace Solid.Identity.Protocols.Saml2p.Middleware
{
    internal class CompleteSsoEndpointMiddleware : IdentityProviderEndpointMiddleware<string>
    {
        private Saml2pSerializer _serializer;
        private IRazorPageRenderingService _razor;
        private Saml2SecurityTokenHandler _handler;
        private ISecurityTokenDescriptorFactory _descriptorFactory;
        private SamlResponseFactory _responseFactory;

        public CompleteSsoEndpointMiddleware(Saml2pSerializer serializer, IRazorPageRenderingService razor, Saml2SecurityTokenHandler handler, ISecurityTokenDescriptorFactory descriptorFactory, SamlResponseFactory responseFactory, Saml2pCache cache, string idpId, IOptionsMonitor<Saml2pIdentityProviderOptions> monitor, ILoggerFactory loggerFactory, RequestDelegate next)
            : base(idpId, cache, monitor, loggerFactory, next)
        {
            _serializer = serializer;
            _razor = razor;
            _handler = handler;
            _descriptorFactory = descriptorFactory;
            _responseFactory = responseFactory;
        }

        protected override async ValueTask HandleRequestAsync(HttpContext context, string id)
        {
            Logger.LogInformation("Completing SAML2P authentication.");
            var user = context.User;
            var request = await Cache.FetchRequestAsync(id);
            var partnerId = request.Issuer;
            var partner = IdentityProvider.ServiceProviders.FirstOrDefault(sp => sp.Id == partnerId);

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
                var token = await partner.IdentityProvider.Events.CreateSecurityTokenAsync(context.RequestServices, createSecurityTokenContext);
                var response = _responseFactory.Create(partner, authnRequestId: request.Id, token: token);
                var xml = _serializer.SerializeSamlResponse(response);
                var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(xml));
                var model = new SamlResponseModel
                {
                    Recipient = new Uri(partner.BaseUrl, partner.AssertionConsumerServiceEndpoint),
                    SamlResponse = base64,
                    // TODO: RelayState = 
                };
                var html = await _razor.RenderPageAsync(model, "SamlResponse", "__Saml2p");
                context.Response.StatusCode = 200;
                context.Response.ContentType = "text/html";
                var bytes = Encoding.UTF8.GetBytes(html);
                await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
            }
            else
            {
                // 401?
                // maybe another event
            }
        }

        protected override bool IsValidRequest(HttpContext context, out string id)
        {
            id = context.Request.Query["id"];
            return id != null;
        }
    }
}
