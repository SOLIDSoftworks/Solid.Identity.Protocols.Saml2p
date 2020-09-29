using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Authentication
{
    public class Saml2pAuthenticationOptions : RemoteAuthenticationOptions, ISaml2pAuthenticationOptions
    {
        public string IdentityProviderId { get; set; }
    }

    public interface ISaml2pAuthenticationOptions
    {
        string IdentityProviderId { get; set; }
    }
}
