using Solid.Identity.Protocols.Saml2p.Options;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Solid.Identity.Protocols.Saml2p.Abstractions;

namespace Solid.Identity.Protocols.Saml2p.Models.Context
{
    /// <summary>
    /// A context class used with <see cref="Saml2pIdentityProviderEvents.OnAcceptSso"/>.
    /// </summary>
    public class AcceptSsoContext
    {
        /// <summary>
        /// The partner id.
        /// </summary>
        public string PartnerId { get; internal set; }

        /// <summary>
        /// The Saml2p SP partner.
        /// </summary>
        public ISaml2pServiceProvider Partner { get; internal set; }

        /// <summary>
        /// The <see cref="AuthnRequest"/> that wes received from the partner SP.
        /// </summary>
        public AuthnRequest Request { get; internal set; }

        /// <summary>
        /// The return URL.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// The authentication scheme to be used to authenticate the user.
        /// </summary>
        public string AuthenticationScheme { get; set; }

        /// <summary>
        /// The authentication property items that can be used during the challenge.
        /// </summary>
        public IDictionary<string, string> AuthenticationPropertyItems { get; } = new Dictionary<string, string>();
    }
}
