using Solid.Identity.Protocols.Saml2p.Options;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Models.Context
{
    public class StartSsoContext
    {
        public string PartnerId { get; set; }
        public PartnerSaml2pIdentityProvider Partner { get; set; }
        public AuthnRequest AuthnRequest { get; internal set; }
    }
}
