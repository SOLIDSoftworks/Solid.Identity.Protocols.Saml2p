using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens.Saml2;
using Solid.Identity.Protocols.Saml2p.Cache;
using Solid.Identity.Protocols.Saml2p.Factories;
using Solid.Identity.Protocols.Saml2p.Models.Context;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using Solid.Identity.Protocols.Saml2p.Models.Results;
using Solid.Identity.Protocols.Saml2p.Options;
using Solid.Identity.Protocols.Saml2p.Providers;
using Solid.Identity.Protocols.Saml2p.Serialization;
using Solid.Identity.Protocols.Saml2p.Services;
using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.Saml2p.Middleware.Sp
{
    internal class FinishSsoEndpointMiddleware : Saml2pEndpointMiddleware
    {
        private readonly Saml2SecurityTokenHandler _handler;
        private readonly TokenValidationParametersFactory _factory;
        
#if NET6_0
        private readonly ISystemClock _clock;

        public FinishSsoEndpointMiddleware(
            Saml2SecurityTokenHandler handler,
            TokenValidationParametersFactory parametersFactory,
            ISystemClock clock,
            Saml2pSerializer serializer,
            Saml2pCache cache,
            Saml2pPartnerProvider partners,
            Saml2pEncodingService encoder,
            IOptionsMonitor<Saml2pOptions> monitor,
            ILoggerFactory factory,
            RequestDelegate _)
            : this(handler, parametersFactory, clock, serializer, cache, partners, encoder, monitor, factory)
        {
        }

        public FinishSsoEndpointMiddleware(
            Saml2SecurityTokenHandler handler,
            TokenValidationParametersFactory parametersFactory,
            ISystemClock clock,
            Saml2pSerializer serializer,
            Saml2pCache cache,
            Saml2pPartnerProvider partners,
            Saml2pEncodingService encoder,
            IOptionsMonitor<Saml2pOptions> monitor,
            ILoggerFactory factory)
            : base(serializer, cache, partners, encoder, monitor, factory)
        {
            _handler = handler;
            _factory = parametersFactory;
            _clock = clock;
        }
#else
        private readonly TimeProvider _time;

        public FinishSsoEndpointMiddleware(
            Saml2SecurityTokenHandler handler,
            TokenValidationParametersFactory parametersFactory,
            TimeProvider time,
            Saml2pSerializer serializer,
            Saml2pCache cache,
            Saml2pPartnerProvider partners,
            Saml2pEncodingService encoder,
            IOptionsMonitor<Saml2pOptions> monitor,
            ILoggerFactory factory,
            RequestDelegate _)
            : this(handler, parametersFactory, time, serializer, cache, partners, encoder, monitor, factory)
        {
        }

        public FinishSsoEndpointMiddleware(
            Saml2SecurityTokenHandler handler,
            TokenValidationParametersFactory parametersFactory,
            TimeProvider time,
            Saml2pSerializer serializer,
            Saml2pCache cache,
            Saml2pPartnerProvider partners,
            Saml2pEncodingService encoder,
            IOptionsMonitor<Saml2pOptions> monitor,
            ILoggerFactory factory)
            : base(serializer, cache, partners, encoder, monitor, factory)
        {
            _handler = handler;
            _factory = parametersFactory;
            _time = time;
        }
        
#endif

        public override async Task InvokeAsync(HttpContext context)
        {
            try
            {
                var result = await FinishSsoAsync(context);
                if (!result.IsSuccessful)
                {
                    context.Response.StatusCode = 401;
                    return;
                }

                await context.SignInAsync(result.Subject, result.Properties);
                context.Response.Redirect(Options.DefaultRedirectPath);
            }
            catch(InvalidOperationException)
            {
                context.Response.StatusCode = 400;
            }
        }

        public async Task<FinishSsoResult> FinishSsoAsync(HttpContext context)
        {
            using var activity = CreateActivity(nameof(FinishSsoAsync));
            if (!TryGetSamlResponse(context, out var response, out var binding))
            {
                throw new InvalidOperationException("Bad request.");
            }

            Logger.LogInformation("Finishing SAML2P authentication (SP flow).");
            Trace($"Received SAMLResponse using {binding} binding.", response);
            var partnerId = response.Issuer;
            var partner = await Partners.GetIdentityProviderAsync(partnerId);

            if (partner == null)
                throw new SecurityException($"Partner idp '{partnerId}' not found.");

            if (!partner.Enabled)
                throw new SecurityException($"Partner idp '{partnerId}' is disabled.");

            if (partner.SamlResponseSigningKey != null)
            {
                if(response.Signature == null)
                    throw new SecurityException($"Partner idp '{partnerId}' is expected to sign the SAMLResponse, but no signature was found.");
                if(response.Signature.KeyInfo.KeyName != partner.SamlResponseSigningKey.KeyId)
                    throw new SecurityException($"SAMLResponse fro partner idp '{partnerId}' signed by unexpected key.");
            }
            
            var request = null as AuthnRequest;

            if (response.InResponseTo != null)
            {
                request = await Cache.FetchRequestAsync(response.InResponseTo);
                await Cache.RemoveAsync(response.InResponseTo);
            }

            if (request == null && !partner.CanInitiateSso)
                throw new SecurityException($"Partner idp '{partnerId}' is is not allowed to initiate SSO.");

            if (request != null)
            {
                Trace("Found cached SAMLRequest.", request);

                if (request.RelayState != response.RelayState)
                    throw new SecurityException($"Mismatching relay state.");
            }

            var ssoContext = new FinishSsoContext
            {
                PartnerId = partner.Id,
                Partner = partner,
                Request = request,
                Response = response
            };

            await Events.InvokeAsync(Options, partner, e => e.OnFinishSso(context.RequestServices, ssoContext));

            if (response.Status.StatusCode.Value != Saml2pConstants.Statuses.Success)
            {
                return FinishSsoResult.Fail(partner.Id, response.Status.StatusCode.Value, response.Status.StatusCode?.SubCode.Value);
            }
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

            if (validateContext.Subject != null && validateContext.SecurityToken == null ||
                validateContext.Subject == null && validateContext.SecurityToken != null)
            {
                Logger.LogWarning($"When manually populating '{nameof(ValidateTokenContext.Subject)}' or '{nameof(ValidateTokenContext.SecurityToken)}' properties of '{nameof(ValidateTokenContext)}', then both must be populated. Otherwise they will be ignored. Clearing values...");
                validateContext.SecurityToken = null;
                validateContext.Subject = null;
            }

            if (validateContext.Subject == null)
            {
                Logger.LogInformation("Validating incoming token.");
                var subject = validateContext.Handler.ValidateToken(validateContext.Response.XmlSecurityToken, validateContext.TokenValidationParameters, out var token);
                var saml2 = token as Saml2SecurityToken;
                var now = GetUtcNow();

                saml2.ValidateResponseToken(validateContext.Request?.Id, now);

                validateContext.Subject = subject;
                validateContext.SecurityToken = saml2;
            }

            await Events.InvokeAsync(Options, partner, e => e.OnValidatedToken(context.RequestServices, validateContext));

            context.User = validateContext.Subject;

            var properties = new AuthenticationProperties();
            if (validateContext.Subject != null)
            {
                properties.IssuedUtc = validateContext.SecurityToken!.ValidFrom;
                properties.ExpiresUtc = validateContext.SecurityToken!.ValidTo;
                
                var authn = new AuthenticationToken
                {
                    Name = Saml2pConstants.TokenName,
                    Value = validateContext.Response.XmlSecurityToken
                };
                properties.StoreTokens(new[] { authn });
            }

            return FinishSsoResult.Success(partner.Id, validateContext.Response.XmlSecurityToken, validateContext.SecurityToken, validateContext.Subject, properties);
        }
        

        private DateTime GetUtcNow()
        {
#if NET6_0
            return _clock.UtcNow.UtcDateTime;
#else
            return _time.GetUtcNow().UtcDateTime;
#endif
        }
    }
}
