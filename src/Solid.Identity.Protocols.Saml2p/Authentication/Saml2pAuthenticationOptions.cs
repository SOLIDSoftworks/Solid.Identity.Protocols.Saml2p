using Microsoft.AspNetCore.Authentication;
using Solid.Identity.Protocols.Saml2p.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Authentication
{
    /// <summary>
    /// The options for Saml2p authentication.
    /// </summary>
    public class Saml2pAuthenticationOptions : RemoteAuthenticationOptions, ISaml2pAuthenticationOptions
    {
        /// <summary>
        /// Default constructor for <see cref="Saml2pAuthenticationOptions"/>.
        /// </summary>
        public Saml2pAuthenticationOptions() => CallbackPath = "/not_used";
        /// <summary>
        /// The id of the identity provider where authentication will be performed.
        /// </summary>
        public string IdentityProviderId { get; set; }
    }
}
