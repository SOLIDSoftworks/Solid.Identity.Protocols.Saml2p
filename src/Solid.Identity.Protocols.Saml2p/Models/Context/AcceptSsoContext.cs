﻿using Solid.Identity.Protocols.Saml2p.Options;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Solid.Identity.Protocols.Saml2p.Abstractions;

namespace Solid.Identity.Protocols.Saml2p.Models.Context
{
    public class AcceptSsoContext
    {
        public string PartnerId { get; internal set; }
        public ISaml2pServiceProvider Partner { get; internal set; }
        public AuthnRequest Request { get; internal set; }
        public ClaimsPrincipal User { get; internal set; }
        public string ReturnUrl { get; internal set; }
        public string RelayState { get; internal set; }
    }
}
