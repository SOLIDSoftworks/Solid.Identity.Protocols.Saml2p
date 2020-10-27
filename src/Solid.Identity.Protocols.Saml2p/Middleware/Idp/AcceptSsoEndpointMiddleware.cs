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
using Solid.Identity.Protocols.Saml2p.Serialization;
using System.Linq;
using System.Security;
using Solid.Identity.Protocols.Saml2p.Cache;
using Solid.Identity.Protocols.Saml2p.Models.Context;
using Solid.Identity.Protocols.Saml2p.Providers;
using Solid.Identity.Protocols.Saml2p.Services;

namespace Solid.Identity.Protocols.Saml2p.Middleware.Idp
{
    internal class AcceptSsoEndpointMiddleware : Saml2pEndpointMiddleware
    {
        public AcceptSsoEndpointMiddleware(
            Saml2pSerializer serializer, 
            Saml2pCache cache, 
            Saml2pPartnerProvider partners, 
            Saml2pEncodingService encoder,
            IOptionsMonitor<Saml2pOptions> monitor, 
            ILoggerFactory loggerFactory, 
            RequestDelegate _) : base(serializer, cache, partners, encoder, monitor, loggerFactory)
        {
        }

        public override async Task InvokeAsync(HttpContext context)
        {
            if(!TryGetAuthnRequest(context, out var request, out var binding))
            {
                context.Response.StatusCode = 400;
                return;
            }
            
            Logger.LogInformation("Accepting SAML2P authentication (IDP flow).");
            Trace($"Received SAMLRequest using {binding} binding.", request);
            var partnerId = request.Issuer;
            var partner = await Partners.GetServiceProviderAsync(partnerId);

            if (partner == null)
                throw new SecurityException($"Partner '{partnerId}' not found.");

            if (!partner.Enabled)
                throw new SecurityException($"Partner '{partnerId}' is disabled.");

            if (!partner.CanInitiateSso)
                throw new SecurityException($"Partner '{partnerId}' is not allowed to initiate SSO.");

            await Cache.CacheRequestAsync(request.Id, request);
            var ssoContext = new AcceptSsoContext
            {
                PartnerId = partner.Id,
                Partner = partner,
                Request = request,
                ReturnUrl = GenerateReturnUrl(context, request.Id)
            };

            await Events.InvokeAsync(Options, partner, e => e.OnAcceptSso(context.RequestServices, ssoContext));

            if(ssoContext.AuthenticationScheme != null)
                await ChallengeAsync(context, request, ssoContext.ReturnUrl, ssoContext.AuthenticationScheme);
            else
                await ChallengeAsync(context, request, ssoContext.ReturnUrl);
        }
    }
}
