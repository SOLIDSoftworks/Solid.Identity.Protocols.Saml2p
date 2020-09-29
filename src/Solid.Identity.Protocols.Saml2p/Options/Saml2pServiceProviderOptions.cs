using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Options
{
    public class Saml2pServiceProviderOptions : Saml2pServiceProvider
    {
        public ICollection<PartnerSaml2pIdentityProvider> IdentityProviders { get; internal set; } = new List<PartnerSaml2pIdentityProvider>();
        public Saml2pServiceProviderEvents Events { get; } = new Saml2pServiceProviderEvents();
    }
}
