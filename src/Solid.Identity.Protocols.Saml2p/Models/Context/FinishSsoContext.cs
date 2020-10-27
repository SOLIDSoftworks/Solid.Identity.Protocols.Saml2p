using Solid.Identity.Protocols.Saml2p.Options;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using Solid.Identity.Protocols.Saml2p.Abstractions;

namespace Solid.Identity.Protocols.Saml2p.Models.Context
{
    /// <summary>
    /// A context class used with <see cref="Saml2pServiceProviderEvents.OnFinishSso"/>.
    /// </summary>
    public class FinishSsoContext
    {
        /// <summary>
        /// The partner id.
        /// </summary>
        public string PartnerId { get; internal set; }

        /// <summary>
        /// The Saml2p IDP partner.
        /// </summary>
        public ISaml2pIdentityProvider Partner { get; internal set; }

        /// <summary>
        /// The original <see cref="AuthnRequest"/> that was sent to the IDP.
        /// </summary>
        public AuthnRequest Request { get; internal set; }

        /// <summary>
        /// The <see cref="SamlResponse"/> received from the IDP.
        /// </summary>
        public SamlResponse Response { get; internal set; }
    }
}
