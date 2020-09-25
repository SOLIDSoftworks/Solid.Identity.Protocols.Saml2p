using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Extensions.AspNetCore.Saml2p
{
    public class Saml2pAuthenticationOptions : RemoteAuthenticationOptions, ISaml2pAuthenticationOptions
    {
        public string PartnerId { get; set; }
    }

    public interface ISaml2pAuthenticationOptions
    {
        string PartnerId { get; set; }
    }
}
