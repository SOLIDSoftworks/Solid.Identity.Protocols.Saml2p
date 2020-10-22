using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens.Saml2;
using Solid.Identity.Protocols.Saml2p.Cache;
using Solid.Identity.Protocols.Saml2p.Factories;
using Solid.Identity.Protocols.Saml2p.Models.Context;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using Solid.Identity.Protocols.Saml2p.Options;
using Solid.Identity.Protocols.Saml2p.Providers;
using Solid.Identity.Protocols.Saml2p.Serialization;
using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.Saml2p.Middleware.Sp
{
    internal class FinishSsoEndpointMiddleware : Saml2pEndpointMiddleware
    {
        private Saml2SecurityTokenHandler _handler;
        private TokenValidationParametersFactory _factory;

        public FinishSsoEndpointMiddleware(Saml2SecurityTokenHandler handler, TokenValidationParametersFactory parametersFactory, Saml2pSerializer serializer, Saml2pCache cache, Saml2pPartnerProvider partners, IOptionsMonitor<Saml2pOptions> monitor, ILoggerFactory factory, RequestDelegate _)
            : this(handler, parametersFactory, serializer, cache, partners, monitor, factory)
        {
        }

        public FinishSsoEndpointMiddleware(Saml2SecurityTokenHandler handler, TokenValidationParametersFactory parametersFactory, Saml2pSerializer serializer, Saml2pCache cache, Saml2pPartnerProvider partners, IOptionsMonitor<Saml2pOptions> monitor, ILoggerFactory factory)
            : base(serializer, cache, partners, monitor, factory)
        {
            _handler = handler;
            _factory = parametersFactory;
        }

        public override async Task InvokeAsync(HttpContext context)
        {
            if (!TryGetSamlResponse(context, out var response, out var binding))
            {
                context.Response.StatusCode = 400;
                return;
            }

            Logger.LogInformation("Finishing SAML2P authentication (SP flow).");
            Trace($"Received SAMLResponse using {binding} binding.", response);
            var partnerId = response.Issuer;
            var partner = await Partners.GetIdentityProviderAsync(partnerId);

            if (partner == null)
                throw new SecurityException($"Partner idp '{partnerId}' not found.");

            if (!partner.Enabled)
                throw new SecurityException($"Partner idp '{partnerId}' is disabled.");

            var request = null as AuthnRequest;

            if (response.InResponseTo != null)
                request = await Cache.FetchRequestAsync(response.InResponseTo);

            if (request == null && !partner.CanInitiateSso)
                throw new SecurityException($"Partner idp '{partnerId}' is is not allowed to initiate SSO.");
            
            Trace("Found cached SAMLRequest.", request);

            if (request.RelayState != response.RelayState)
                throw new SecurityException($"Mismatching relay state.");

            var ssoContext = new FinishSsoContext
            {
                PartnerId = partner.Id,
                Partner = partner,
                Request = request,
                Response = response
            };

            await Events.InvokeAsync(Options, partner, e => e.OnFinishSso(context.RequestServices, ssoContext));

            var parameters = _factory.Create(partner);
            var validateContext = new ValidateTokenContext
            {
                PartnerId = partner.Id,
                Partner = partner,
                Request = request,
                Response = response,
                TokenValidationParameters = parameters,
                Handler = _handler
            };

            await Events.InvokeAsync(Options, partner, e => e.OnValidatingToken(context.RequestServices, validateContext));

            if(validateContext.Subject == null)
            {
                var subject = validateContext.Handler.ValidateToken(validateContext.Response.XmlSecurityToken, validateContext.TokenValidationParameters, out _);
                validateContext.Subject = subject;
            }

            await Events.InvokeAsync(Options, partner, e => e.OnValidatedToken(context.RequestServices, validateContext));

            context.User = validateContext.Subject;

        }
    }
}
