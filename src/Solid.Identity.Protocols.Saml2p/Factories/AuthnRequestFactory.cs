using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Solid.Identity.Protocols.Saml2p.Configuration;
using Solid.Identity.Protocols.Saml2p.Models;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Factories
{
    public class AuthnRequestFactory
    {
        private ISystemClock _systemClock;

        public AuthnRequestFactory(ISystemClock systemClock)
        {
            _systemClock = systemClock;
        }

        public AuthnRequest CreateAuthnRequest(HttpContext context, PartnerSaml2pIdentityProvider idp)
        {
            var request = context.Request;
            var acs = idp.ServiceProvider.AssertionConsumerServiceUrl;
            if (!acs.IsAbsoluteUri)
                acs = new Uri(GetBaseUrl(request), acs);
            return new AuthnRequest
            {
                Id = $"_{Guid.NewGuid()}",
                ProviderName = idp.Id,
                AssertionConsumerServiceUrl = acs,
                IssueInstant = _systemClock.UtcNow.UtcDateTime,
                Issuer = idp.ServiceProvider.Id,
                Destination = idp.SsoEndpoint,
                NameIdPolicy = new NameIdPolicy
                {
                    Format = idp.NameIdPolicyFormat
                },
                RequestedAuthnContext = new RequestedAuthnContext
                {
                    AuthnContextClassRef = idp.RequestedAuthnContextClassRef
                }
            };
        }

        private Uri GetBaseUrl(HttpRequest request) => new Uri($"{request.Scheme}://{request.Host}");
    }
}
