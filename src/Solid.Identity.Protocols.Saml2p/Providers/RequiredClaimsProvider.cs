using Solid.Identity.Protocols.Saml2p.Abstractions;
using Solid.Identity.Protocols.Saml2p.Models;
using Solid.Identity.Protocols.Saml2p.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.Saml2p.Providers
{
    internal class RequiredClaimsProvider : IServiceProviderClaimsProvider
    {
        private Saml2pPartnerProvider _partnerProvider;

        public RequiredClaimsProvider(Saml2pPartnerProvider partnerProvider) => _partnerProvider = partnerProvider;

        public IEnumerable<ClaimDescriptor> ClaimTypesOffered => new[]
        {
            new ClaimDescriptor(ClaimTypes.NameIdentifier),
            new ClaimDescriptor(ClaimTypes.AuthenticationInstant),
            new ClaimDescriptor(ClaimTypes.AuthenticationMethod)
        };

        public async ValueTask<bool> CanGenerateClaimsAsync(string partnerId)
        {
            var partner = await _partnerProvider.GetServiceProviderAsync(partnerId);
            return !partner.AllowClaimsPassthrough;
        }

        public ValueTask<IEnumerable<Claim>> GetClaimsAsync(ClaimsIdentity identity, ISaml2pServiceProvider _, string issuer)
        {
            var claims = new List<Claim>();
            var sub = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(sub))
                claims.Add(new Claim(ClaimTypes.NameIdentifier, sub, null, issuer));
            var instant = identity.FindFirst(ClaimTypes.AuthenticationInstant)?.Value;
            if (!string.IsNullOrEmpty(instant))
                claims.Add(new Claim(ClaimTypes.AuthenticationInstant, instant, null, issuer));
            var method = identity.FindFirst(ClaimTypes.AuthenticationMethod)?.Value;
            if (!string.IsNullOrEmpty(method))
                claims.Add(new Claim(ClaimTypes.AuthenticationMethod, method, null, issuer));
            return new ValueTask<IEnumerable<Claim>>(claims);
        }
    }
}

