using Solid.Identity.Protocols.Saml2p.Abstractions;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using Solid.Identity.Protocols.Saml2p.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Models.Context
{
    public class GenerateRelayStateContext
    {
        public string PartnerId { get; internal set; }
        public ISaml2pIdentityProvider Partner { get; internal set; }
        public AuthnRequest Request { get; internal set; }
        public string RelayState { get => Request.RelayState; set => Request.RelayState = value; }
    }
}
