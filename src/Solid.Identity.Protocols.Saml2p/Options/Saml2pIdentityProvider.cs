using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Options
{
    public class Saml2pIdentityProvider : Saml2pProvider
    {
        public bool CanInitiateSso { get; set; }
        public bool WantsAuthnRequestsSigned { get; set; }
    }
}
