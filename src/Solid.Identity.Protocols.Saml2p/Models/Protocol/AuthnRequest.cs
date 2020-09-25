using Microsoft.IdentityModel.Tokens.Saml2;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Models.Protocol
{
    public class AuthnRequest
    {
        /// <summary>
        /// The id of this AuthnRequest.
        /// <para>Cannot start with a number.</para>
        /// </summary>
        public string Id { get; set; }
        public string Version { get; set; } = Saml2Constants.Version;
        public DateTime? IssueInstant { get; set; }
        public Uri AssertionConsumerServiceUrl { get; set; }
        public bool? ForceAuthn { get; set; }
        public bool? IsPassive { get; set; }
        /// <summary>
        /// The issuer of this AuthnRequest (the service provider)
        /// </summary>
        public string Issuer { get; set; }
        public string ProviderName { get; set; }
        public Uri Destination { get; set; }
        public string ProtocolBinding { get; set; }

        public NameIdPolicy NameIdPolicy { get; set; }
        public RequestedAuthnContext RequestedAuthnContext { get; set; }
    }
}
