﻿using Solid.Identity.Protocols.Saml2p.Configuration;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Models.Context
{
    public class TokenValidatedContext
    {
        public string PartnerId { get; internal set; }
        public PartnerSaml2pIdentityProvider Partner { get; internal set; }
        public AuthnRequest Request { get; internal set; }
        public SamlResponse Response { get; internal set; }
        public ClaimsPrincipal Subject { get; set; }
    }
}