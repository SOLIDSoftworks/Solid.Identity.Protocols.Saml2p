using Solid.Identity.Protocols.Saml2p.Options;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using Solid.Identity.Protocols.Saml2p.Abstractions;

namespace Solid.Identity.Protocols.Saml2p.Models.Context
{
    public class StartSsoContext
    {
        public string PartnerId { get; set; }
        public ISaml2pIdentityProvider Partner { get; set; }
        public AuthnRequest AuthnRequest { get; internal set; }
    }
}
