using Solid.Identity.Protocols.Saml2p.Options;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using Solid.Identity.Protocols.Saml2p.Abstractions;

namespace Solid.Identity.Protocols.Saml2p.Models.Context
{
    /// <summary>
    /// A context class used with <see cref="Saml2pServiceProviderEvents.OnStartSso"/>.
    /// </summary>
    public class StartSsoContext
    {
        /// <summary>
        /// The partner id.
        /// </summary>
        public string PartnerId { get; set; }

        /// <summary>
        /// The Saml2p IDP partner.
        /// </summary>
        public ISaml2pIdentityProvider Partner { get; set; }

        /// <summary>
        /// The <see cref="AuthnRequest"/> to be sent to the IDP.
        /// </summary>
        public AuthnRequest Request { get; internal set; }
    }
}
