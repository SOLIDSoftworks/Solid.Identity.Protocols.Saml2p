using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Solid.Identity.Protocols.Saml2p.Options;
using Solid.Identity.Protocols.Saml2p.Models;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using Solid.Identity.Protocols.Saml2p.Models.Context;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.Saml2p.Factories
{
    public class AuthnRequestFactory
    {
        private ISystemClock _systemClock;
        private Saml2pOptions _options;

        public AuthnRequestFactory(ISystemClock systemClock, IOptions<Saml2pOptions> options)
        {
            _systemClock = systemClock;
            _options = options.Value;
        }

        public async Task<AuthnRequest> CreateAuthnRequestAsync(HttpContext context, ISaml2pIdentityProvider idp)
        {
            var request = new AuthnRequest
            {
                Id = $"_{Guid.NewGuid()}",
                ProviderName = idp.Id,
                AssertionConsumerServiceUrl = GetAcsUrl(context.Request),
                IssueInstant = _systemClock.UtcNow.UtcDateTime,
                Issuer = idp.ExpectedIssuer ?? _options.Issuer,
                Destination = new Uri(idp.BaseUrl, idp.SsoEndpoint),
                NameIdPolicy = new NameIdPolicy
                {
                    Format = idp.NameIdPolicyFormat
                },
                RequestedAuthnContext = new RequestedAuthnContext
                {
                    AuthnContextClassRef = idp.RequestedAuthnContextClassRef
                }
            };
            var generateContext = new GenerateRelayStateContext
            {
                Partner = idp,
                PartnerId = idp.Id,
                Request = request
            };

            await _options.OnGeneratingRelayState(context.RequestServices, generateContext);
            await idp.OnGeneratingRelayState(context.RequestServices, generateContext);

            if (request.RelayState == null)
                request.RelayState = Guid.NewGuid().ToString();

            return request;
        }

        private Uri GetAcsUrl(HttpRequest request)
        {
            var baseUrl = new Uri($"{request.Scheme}://{request.Host}");

            // TODO: add central utility for creating all paths
            var path = request.PathBase.Add("/acs");
            return new Uri(baseUrl, path);
        }
    }
}
