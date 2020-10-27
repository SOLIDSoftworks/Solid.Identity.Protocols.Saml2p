using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Models.Protocol
{
    /// <summary>
    /// The &lt;NameIDPolicy> element tailors the name identifier in the subjects of assertions resulting from an
    /// &lt;AuthnRequest>.
    /// </summary>
    public class NameIdPolicy
    {
        /// <summary>
        /// A Boolean value used to indicate whether the identity provider is allowed, in the course of fulfilling the
        /// request, to create a new identifier to represent the principal.Defaults to "false"
        /// </summary>
        public bool? AllowCreate { get; set; }

        /// <summary>
        /// Specifies the URI reference corresponding to a name identifier format.
        /// </summary>
        public string Format { get; set; }
    }
}
