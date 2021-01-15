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
        public IEnumerable<ClaimDescriptor> ClaimTypesOffered => Enumerable.Empty<ClaimDescriptor>();

        public ValueTask<bool> CanGenerateClaimsAsync(ISaml2pServiceProvider partner) => new ValueTask<bool>(partner.AllowClaimsPassthrough);

        public ValueTask<IEnumerable<Claim>> GenerateClaimsAsync(ClaimsIdentity identity, ISaml2pServiceProvider _, string __)
            => new ValueTask<IEnumerable<Claim>>(identity.Claims);
    }
}
