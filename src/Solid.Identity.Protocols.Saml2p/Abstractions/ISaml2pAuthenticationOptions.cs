using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Abstractions
{
    public interface ISaml2pAuthenticationOptions
    {
        string IdentityProviderId { get; set; }
    }
}
