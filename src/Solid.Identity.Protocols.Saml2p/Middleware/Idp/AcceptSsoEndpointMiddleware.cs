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
using Solid.Identity.Protocols.Saml2p.Models;
using Microsoft.IdentityModel.Tokens.Saml2;

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

        public override Task InvokeAsync(HttpContext context)
            => AcceptSsoAsync(context);

        private async Task AcceptSsoAsync(HttpContext context)
        {
            using var activity = CreateActivity(nameof(AcceptSsoAsync));
            if(!TryGetAuthnRequest(context, out var request, out var binding))
            {
                context.Response.StatusCode = 400;
                return;
            }
            
            Logger.LogInformation("Accepting SAML2P authentication (IDP flow).");
            Trace($"Received SAMLRequest using {binding} binding.", request);
            var partner = await Partners.GetServiceProviderAsync(request.Issuer);

            if (partner == null)
                throw new SecurityException($"Partner '{request.Issuer}' not found.");

            //if (!partner.Enabled)
            //    throw new SecurityException($"Partner '{partnerId}' is disabled.");

            //if (!partner.CanInitiateSso)
            //    throw new SecurityException($"Partner '{partnerId}' is not allowed to initiate SSO.");

            await Cache.CacheRequestAsync(request.Id, request);
            var ssoContext = new AcceptSsoContext
            {
                PartnerId = request.Issuer,
                Partner = partner,
                Request = request,
                ReturnUrl = GenerateReturnUrl(context, request.Id)
            };

            await Events.InvokeAsync(Options, partner, e => e.OnAcceptSso(context.RequestServices, ssoContext));
            
            if (!IsValid(ssoContext, out var status, out var subStatus) && status.HasValue)
            {
                Logger.LogWarning($"SAMLRequest failed validation. Resulting error: '{(subStatus ?? status)}'");
                await Cache.CacheStatusAsync(request.Id, status.Value.ToStatus(subStatus));
                context.Response.Redirect(ssoContext.ReturnUrl);
            }
            else if (ssoContext.AuthenticationScheme != null)
                await ChallengeAsync(context, request, ssoContext.ReturnUrl, ssoContext.AuthenticationPropertyItems, ssoContext.AuthenticationScheme);
            else
                await ChallengeAsync(context, request, ssoContext.ReturnUrl, ssoContext.AuthenticationPropertyItems);
        }

        // TODO: extract this to a validator class and test it
        private bool IsValid(AcceptSsoContext context, out SamlResponseStatus? status, out SamlResponseStatus? subStatus)
        {
            if(context.Request.Version != Saml2Constants.Version)
            {
                status = SamlResponseStatus.VersionMismatch;
                subStatus = null;
                return false;
            }

            if (!string.IsNullOrEmpty(context.Request.ProtocolBinding) && 
                Options.SupportedBindings.Select(b => b.ToProtocolBindingString()).Any(b => b == context.Request.ProtocolBinding))
            {
                status = SamlResponseStatus.Requester;
                subStatus = SamlResponseStatus.UnsupportedBinding;
                return false;
            }

            if (context.Partner == null)
            {
                status = SamlResponseStatus.Requester;
                subStatus = SamlResponseStatus.RequestDenied;
                return false;
            }

            if (!context.Partner.Enabled || !context.Partner.CanInitiateSso)
            {
                status = SamlResponseStatus.Requester;
                subStatus = SamlResponseStatus.RequestDenied;
                return false;
            }

            if (!context.Partner.SupportedBindings.Any())
            {
                status = SamlResponseStatus.Responder;
                subStatus = SamlResponseStatus.UnsupportedBinding;
                return false;
            }

            status = null;
            subStatus = null;
            return true;
        }
    }
}
