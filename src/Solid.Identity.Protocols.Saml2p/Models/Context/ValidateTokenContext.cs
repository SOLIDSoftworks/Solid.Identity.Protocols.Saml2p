using Solid.Identity.Protocols.Saml2p.Options;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml2;

namespace Solid.Identity.Protocols.Saml2p.Models.Context
{
    public class ValidateTokenContext
    {
        public string PartnerId { get; internal set; }
        public PartnerSaml2pIdentityProvider Partner { get; internal set; }
        public AuthnRequest Request { get; internal set; }
        public SamlResponse Response { get; internal set; }
        public TokenValidationParameters TokenValidationParameters { get; internal set; }
        public Saml2SecurityTokenHandler Handler { get; internal set; }
    }
}
