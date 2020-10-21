using Solid.Identity.Protocols.Saml2p.Options;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using Solid.Identity.Protocols.Saml2p.Abstractions;

namespace Solid.Identity.Protocols.Saml2p.Models.Context
{
    public class FinishSsoContext
    {
        public string PartnerId { get; internal set; }
        public ISaml2pIdentityProvider Partner { get; internal set; }
        public AuthnRequest Request { get; internal set; }
        public SamlResponse Response { get; internal set; }
    }
}
