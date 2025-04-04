using Solid.Identity.Protocols.Saml2p.Models;
using Solid.Identity.Protocols.Saml2p.Options;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Solid.Identity.Protocols.Saml2p.Abstractions
{
    /// <summary>
    /// An interface describing the base of a SAML2P SSO partner.
    /// </summary>
    /// <typeparam name="TEvents">A type containing events to be run.</typeparam>
    public interface ISaml2pPartner<TEvents>
    {
        /// <summary>
        /// The id of the partner. 
        /// </summary>
        string Id { get; }

        /// <summary>
        /// The issuer id that the partner expects. If it is null, <see cref="Saml2pOptions.DefaultIssuer"/> is used.
        /// </summary>
        string ExpectedIssuer { get; }

        /// <summary>
        /// The name of the partner.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The base url for the parnter service.
        /// </summary>
        Uri BaseUrl { get; }

        /// <summary>
        /// A flag tindicating whether the partner is enabled or not.
        /// </summary>
        bool Enabled { get; }

        /// <summary>
        /// A flag indicating whether the partner can initiate SSO.
        /// </summary>
        bool CanInitiateSso { get; }

        /// <summary>
        /// The bindings supported by the partner.
        /// </summary>
        ICollection<BindingType> SupportedBindings { get; }

        /// <summary>
        /// The events object.
        /// </summary>
        TEvents Events { get; }
        
        /// <summary>
        /// Signing key used to sign and validate SAMLResponse
        /// </summary>
        SecurityKey SamlResponseSigningKey { get; }
        
        /// <summary>
        /// Signing key used to sign and validate AuthnRequest
        /// </summary>
        SecurityKey AuthnRequestSigningKey { get; }
    }
}
