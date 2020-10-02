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
using System.Linq;
using System.Security;
using Solid.Identity.Protocols.Saml2p.Cache;
using Solid.Identity.Protocols.Saml2p.Models.Context;

namespace Solid.Identity.Protocols.Saml2p.Middleware
{
    internal class InitiateSsoEndpointMiddleware : IdentityProviderEndpointMiddleware<string>
    {
        public InitiateSsoEndpointMiddleware(string idpId, Saml2pCache cache, IOptionsMonitor<Saml2pIdentityProviderOptions> monitor, ILoggerFactory loggerFactory, RequestDelegate next)
            : base(idpId, cache, monitor, loggerFactory, next)
        {
        }

        protected override async ValueTask HandleRequestAsync(HttpContext context, string partnerId)
        {
            Logger.LogInformation("Initiating SAML2P authentication.");
            var partner = IdentityProvider.ServiceProviders.FirstOrDefault(sp => sp.Id == partnerId);

            if (partner == null)
                throw new SecurityException($"Partner '{partnerId}' not found.");

            if (!partner.Enabled)
                throw new SecurityException($"Partner '{partnerId}' is disabled.");

            if (!partner.IdentityProvider.CanInitiateSso)
                throw new SecurityException($"Local IDP '{partner.IdentityProvider.Id}' is is not allowed to initiate SSO for partner '{partnerId}'.");

            var request = new AuthnRequest
            {
                AssertionConsumerServiceUrl = new Uri(partner.BaseUrl, partner.AssertionConsumerServiceEndpoint),
                Issuer = partner.Id
            };

            var key = $"idp_initiated_{Guid.NewGuid().ToString()}";
            await Cache.CacheRequestAsync(key, request);

            var ssoContext = new InitiateSsoContext
            {
                PartnerId = partner.Id,
                Partner = partner,
                User = context.User,
                ReturnUrl = GenerateReturnUrl(context, key)
            };
            await partner.IdentityProvider.Events.InitiateSsoAsync(context.RequestServices, ssoContext);
        }

        protected override bool IsValidRequest(HttpContext context, out string partnerId)
        {
            partnerId  = context.Request.Query["partnerId"];
            return partnerId != null;
        }
    }
}
