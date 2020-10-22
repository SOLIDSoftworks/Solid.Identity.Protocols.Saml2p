using Solid.Identity.Protocols.Saml2p.Models;
using Solid.Identity.Protocols.Saml2p.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.Saml2p.Abstractions
{
    public interface IServiceProviderClaimsProvider
    {
        ValueTask<bool> CanGenerateClaimsAsync(ISaml2pServiceProvider partner);
        IEnumerable<ClaimDescriptor> ClaimTypesOffered { get; }
        ValueTask<IEnumerable<Claim>> GenerateClaimsAsync(ClaimsIdentity identity, ISaml2pServiceProvider partner, string issuer);
    }
}
