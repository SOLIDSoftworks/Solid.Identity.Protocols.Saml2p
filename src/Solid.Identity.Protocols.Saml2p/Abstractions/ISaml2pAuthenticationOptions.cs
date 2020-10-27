using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Abstractions
{
    /// <summary>
    /// An interface describing the basic options for Saml2p authentication handler.
    /// </summary>
    public interface ISaml2pAuthenticationOptions
    {
        /// <summary>
        /// The id of the IDP to request authentication from.
        /// </summary>
        string IdentityProviderId { get; set; }
    }
}
