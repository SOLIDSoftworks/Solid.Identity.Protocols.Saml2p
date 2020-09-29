using Microsoft.IdentityModel.Tokens;
using Solid.Identity.Protocols.Saml2p.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Models.Context
{
    public class CreateSecurityTokenContext
    {
        public string PartnerId { get; internal set; }
        public PartnerSaml2pServiceProvider Partner { get; internal set; }
        public SecurityTokenDescriptor TokenDescriptor { get; internal set; }
    }
}
