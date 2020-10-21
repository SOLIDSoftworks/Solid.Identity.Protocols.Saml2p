using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Authentication
{
    public class Saml2pAuthenticationOptions : RemoteAuthenticationOptions
    {
        public string IdentityProviderId { get; set; }
    }
}
