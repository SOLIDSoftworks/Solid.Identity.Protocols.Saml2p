using Solid.Identity.Protocols.Saml2p.Abstractions;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using Solid.Identity.Protocols.Saml2p.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Models.Context
{
    /// <summary>
    /// A context class used with <see cref="Saml2pServiceProviderEvents.OnGeneratingRelayState"/>.
    /// </summary>
    public class GenerateRelayStateContext
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
        /// The <see cref="Protocol.AuthnRequest"/> to be sent to the IDP.
        /// </summary>
        public AuthnRequest Request { get; internal set; }

        /// <summary>
        /// The relay state that will be sent to the IDP.
        /// </summary>
        public string RelayState { get => Request.RelayState; set => Request.RelayState = value; }
    }
}
