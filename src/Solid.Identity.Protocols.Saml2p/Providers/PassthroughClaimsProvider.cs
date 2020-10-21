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
    internal class PassthroughClaimsProvider : IServiceProviderClaimsProvider
    {
        private Saml2pPartnerProvider _partnerProvider;

        public PassthroughClaimsProvider(Saml2pPartnerProvider partnerProvider) => _partnerProvider = partnerProvider;

        public IEnumerable<ClaimDescriptor> ClaimTypesOffered => Enumerable.Empty<ClaimDescriptor>();

        public async ValueTask<bool> CanGenerateClaimsAsync(string partnerId)
        {
            var partner = await _partnerProvider.GetServiceProviderAsync(partnerId);
            return partner.AllowClaimsPassthrough;
        }

        public ValueTask<IEnumerable<Claim>> GetClaimsAsync(ClaimsIdentity identity, ISaml2pServiceProvider _, string __)
            => new ValueTask<IEnumerable<Claim>>(identity.Claims);
    }
}
