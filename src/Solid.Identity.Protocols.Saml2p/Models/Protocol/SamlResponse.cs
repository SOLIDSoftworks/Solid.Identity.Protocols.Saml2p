using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml2;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Models.Protocol
{
    public class SamlResponse
    {
        public string Id { get; set; }
        public string Version { get; set; } = Saml2Constants.Version;
        public DateTime? IssueInstant { get; set; }
        public Uri Destination { get; set; }
        public string InResponseTo { get; set; }
        public string Issuer { get; set; }
        public Status Status { get; set; }
        public Saml2SecurityToken SecurityToken { get; set; }
        public string XmlSecurityToken { get; internal set; }
        public string RelayState { get; set; }
    }
}
