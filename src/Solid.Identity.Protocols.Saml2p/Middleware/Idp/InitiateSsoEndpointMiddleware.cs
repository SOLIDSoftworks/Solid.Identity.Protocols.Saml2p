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
using Microsoft.Extensions.Primitives;
using Solid.Identity.Protocols.Saml2p.Providers;

namespace Solid.Identity.Protocols.Saml2p.Middleware.Idp
{
    internal class InitiateSsoEndpointMiddleware : Saml2pEndpointMiddleware
    {
        public InitiateSsoEndpointMiddleware(Saml2pCache cache, Saml2pPartnerProvider partners, IOptionsMonitor<Saml2pOptions> monitor, ILoggerFactory loggerFactory, RequestDelegate _)
            : base(null, cache, partners, monitor, loggerFactory)
        {
        }

        public override async Task InvokeAsync(HttpContext context)
        {
            Logger.LogInformation("Initiating SAML2P authentication (IDP flow).");
            var id = context.Request.Query[Options.PartnerIdQueryParameter];
            if (StringValues.IsNullOrEmpty(id))
                throw new InvalidOperationException($"Missing '{Options.PartnerIdQueryParameter}' query parameter.");
            var partner = await Partners.GetServiceProviderAsync(id);

            if (partner == null)
                throw new SecurityException($"Partner '{id}' not found.");

            if (!partner.Enabled)
                throw new SecurityException($"Partner '{id}' is disabled.");

            if (!partner.AllowsIdpInitiatedSso)
                throw new SecurityException($"IDP initiated SSP is not allowed for partner '{id}'.");

            var request = new AuthnRequest
            {
                AssertionConsumerServiceUrl = new Uri(partner.BaseUrl, partner.AssertionConsumerServiceEndpoint),
                Issuer = partner.Id
            };

            Trace("Generated SAMLRequest.", request);

            var key = $"idp_initiated_{Guid.NewGuid().ToString()}";
            await Cache.CacheRequestAsync(key, request);

            var ssoContext = new InitiateSsoContext
            {
                PartnerId = partner.Id,
                Partner = partner,
                User = context.User,
                ReturnUrl = GenerateReturnUrl(context, key)
            };

            await Options.OnInitiateSso(context.RequestServices, ssoContext);
            await partner.OnInitiateSso(context.RequestServices, ssoContext);

            await ChallengeAsync(context, ssoContext.ReturnUrl);
        }
    }
}
