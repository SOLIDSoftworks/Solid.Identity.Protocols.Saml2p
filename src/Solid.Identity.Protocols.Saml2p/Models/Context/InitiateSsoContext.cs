using Solid.Identity.Protocols.Saml2p.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Models.Context
{
    public class InitiateSsoContext
    {
        public string Id { get; internal set; }
        public string PartnerId { get; internal set; }
        public ClaimsPrincipal User { get; internal set; }
        public PartnerSaml2pServiceProvider Partner { get; internal set; }
        public string ReturnUrl { get; internal set; }
    }
}
