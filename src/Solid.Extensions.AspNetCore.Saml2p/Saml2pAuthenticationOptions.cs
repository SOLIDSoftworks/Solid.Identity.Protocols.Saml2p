using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Extensions.AspNetCore.Saml2p
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
