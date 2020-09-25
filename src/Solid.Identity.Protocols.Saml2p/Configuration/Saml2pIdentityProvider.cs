using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Configuration
{
    public class Saml2pIdentityProvider : Saml2pProvider
    {
        public string Id { get; set; }
        public Uri SsoEndpoint { get; set; }
        public bool CanInitiateSso { get; set; }
        public bool WantsAuthnRequestsSigned { get; set; }
    }
}
