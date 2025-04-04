using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml2;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Xml;

namespace Solid.Identity.Protocols.Saml2p.Models.Protocol
{
    /// <summary>
    /// An authentication response.
    /// </summary>
    public class SamlResponse
    {
        /// <summary>
        /// An identifier for the response.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The version of the response.
        /// </summary>
        /// <value>2.0</value>
        public string Version { get; set; } = Saml2Constants.Version;

        /// <summary>
        /// The time instant of issue of the response. 
        /// </summary>
        public DateTime? IssueInstant { get; set; }

        /// <summary>
        /// A URI reference indicating the address to which this response has been sent. 
        /// </summary>
        public Uri Destination { get; set; }

        /// <summary>
        /// A reference to the identifier of the request to which the response corresponds, if any.
        /// </summary>
        public string InResponseTo { get; set; }

        /// <summary>
        /// Identifies the entity that generated the response message.
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// A code representing the status of the corresponding request.
        /// </summary>
        public Status Status { get; set; }

        /// <summary>
        /// The <see cref="Saml2SecurityToken"/> sent within the <see cref="SamlResponse"/>.
        /// </summary>
        public Saml2SecurityToken SecurityToken { get; set; }

        /// <summary>
        /// The XML representation of the <see cref="Saml2SecurityToken"/> sent within the <see cref="SamlResponse"/>.
        /// </summary>
        public string XmlSecurityToken { get; internal set; }

        /// <summary>
        /// The relay state used to correlate requests and responses.
        /// </summary>
        public string RelayState { get; set; }

        /// <summary>
        /// The signature of the SAMLResponse
        /// </summary>
        public Signature Signature { get; internal set; }

        /// <summary>
        /// Signing credentials used to sign the SAMLResponse
        /// </summary>
        public SigningCredentials SigningCredentials { get; set; }
    }
}
