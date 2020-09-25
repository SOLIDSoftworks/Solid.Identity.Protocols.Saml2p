using Solid.Identity.Protocols.Saml2p.Configuration;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Models.Context
{
    public class FinishSsoContext
    {
        public string PartnerId { get; internal set; }
        public PartnerSaml2pIdentityProvider Partner { get; internal set; }
        public AuthnRequest Request { get; internal set; }
        public SamlResponse Response { get; internal set; }
    }
}
