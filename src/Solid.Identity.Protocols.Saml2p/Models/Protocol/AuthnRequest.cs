using Microsoft.IdentityModel.Tokens.Saml2;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Xml;

namespace Solid.Identity.Protocols.Saml2p.Models.Protocol
{
    /// <summary>
    /// An authentication request.
    /// </summary>
    public class AuthnRequest
    {
        /// <summary>
        /// The id of this AuthnRequest.
        /// <para>Cannot start with a number.</para>
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The version of the protocol.
        /// </summary>
        /// <value>2.0</value>
        public string Version { get; set; } = Saml2Constants.Version;

        /// <summary>
        /// The time instant of issue of the request.
        /// </summary>
        public DateTime? IssueInstant { get; set; }

        /// <summary>
        /// Specifies by value the location to which the &lt;Response> message MUST be returned to the
        /// requester. The responder MUST ensure by some means that the value specified is in fact associated
        /// with the requester.
        /// </summary>
        public Uri AssertionConsumerServiceUrl { get; set; }

        /// <summary>
        ///  If "true", the identity provider MUST authenticate the presenter directly rather than
        /// rely on a previous security context. If a value is not provided, the default is "false".
        /// </summary>
        public bool? ForceAuthn { get; set; }

        /// <summary>
        ///  If "true", the identity provider and the user agent itself MUST NOT visibly take control
        /// of the user interface from the requester and interact with the presenter in a noticeable fashion. If a
        /// value is not provided, the default is "false".
        /// </summary>
        public bool? IsPassive { get; set; }

        /// <summary>
        /// The issuer of this AuthnRequest (the service provider)
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Specifies the human-readable name of the requester for use by the presenter's user agent or the
        /// identity provider.
        /// </summary>
        public string ProviderName { get; set; }

        /// <summary>
        /// A URI reference indicating the address to which this request has been sent.
        /// </summary>
        public Uri Destination { get; set; }

        /// <summary>
        /// A URI reference that identifies a SAML protocol binding to be used when returning the &lt;Response>
        /// message. This attribute is mutually exclusive with the AssertionConsumerServiceIndex attribute
        /// and is typically accompanied by the AssertionConsumerServiceURL attribute.
        /// </summary>
        public string ProtocolBinding { get; set; }

        /// <summary>
        /// Specifies constraints on the name identifier to be used to represent the requested subject. If omitted,
        /// then any type of identifier supported by the identity provider for the requested subject can be used,
        /// constrained by any relevant deployment-specific policies, with respect to privacy, for example.
        /// </summary>
        public NameIdPolicy NameIdPolicy { get; set; }

        /// <summary>
        /// Specifies the requirements, if any, that the requester places on the authentication context that applies
        /// to the responding provider's authentication of the presenter. 
        /// </summary>
        public RequestedAuthnContext RequestedAuthnContext { get; set; }

        /// <summary>
        /// The relay state used to correlate requests and responses.
        /// </summary>
        public string RelayState { get; set; }

        /// <summary>
        /// The signature of the AuthnRequest
        /// </summary>
        public Signature Signature { get; internal set; }

        /// <summary>
        /// Signing credentials used to sign the AuthnRequest
        /// </summary>
        public SigningCredentials SigningCredentials { get; set; }
    }
}
