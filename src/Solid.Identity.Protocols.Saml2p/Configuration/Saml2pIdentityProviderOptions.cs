using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Configuration
{
    public class Saml2pIdentityProviderOptions : Saml2pIdentityProvider
    {
        public Saml2pIdentityProviderEvents Events { get; internal set; } = new Saml2pIdentityProviderEvents();
        public ICollection<PartnerSaml2pServiceProvider> ServiceProviders { get; internal set; } = new List<PartnerSaml2pServiceProvider>();
    }
}
