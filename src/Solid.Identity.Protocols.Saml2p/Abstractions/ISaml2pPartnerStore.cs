using Solid.Identity.Protocols.Saml2p.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.Saml2p.Abstractions
{
    public interface ISaml2pPartnerStore
    {
        ValueTask<ISaml2pIdentityProvider> GetIdentityProviderAsync(string id);
        ValueTask<ISaml2pServiceProvider> GetServiceProviderAsync(string id);
    }
}
