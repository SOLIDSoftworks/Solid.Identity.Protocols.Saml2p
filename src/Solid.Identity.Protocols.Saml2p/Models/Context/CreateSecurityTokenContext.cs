using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml2;
using Solid.Identity.Protocols.Saml2p.Abstractions;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using Solid.Identity.Protocols.Saml2p.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Models.Context
{
    /// <summary>
    /// A context class used with <see cref="Saml2pIdentityProviderEvents.OnCreatingSecurityToken"/> and <see cref="Saml2pIdentityProviderEvents.OnCreatedSecurityToken"/>.
    /// </summary>
    public class CreateSecurityTokenContext
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
        /// The <see cref="SecurityTokenDescriptor"/> used to create <see cref="SecurityToken"/>.
        /// </summary>
        public SecurityTokenDescriptor TokenDescriptor { get; internal set; }

        /// <summary>
        /// The <see cref="Saml2SecurityTokenHandler"/> used to create <see cref="SecurityToken"/>.
        /// </summary>
        public Saml2SecurityTokenHandler Handler { get; internal set; }

        /// <summary>
        /// The <see cref="Saml2SecurityToken"/> that will be sent to the SP within a <see cref="SamlResponse"/>.
        /// </summary>
        public Saml2SecurityToken SecurityToken { get; set; }
    }
}
