using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Models.Protocol
{
    /// <summary>
    /// The &lt;RequestedAuthnContext> element specifies the authentication context requirements of
    /// authentication statements returned in response to a request or query.
    /// </summary>
    public class RequestedAuthnContext
    {
        /// <summary>
        /// Specifies the comparison method used to evaluate the requested context classes or statements.
        /// </summary>
        public Comparison Comparison { get; set; } = Comparison.Exact;

        /// <summary>
        /// A URI reference identifying an authentication context class that describes the authentication context
        /// declaration that follows.
        /// </summary>
        public Uri AuthnContextClassRef { get; set; }

        /// <summary>
        /// Either an authentication context declaration provided by value, or a URI reference that identifies such
        /// a declaration.
        /// </summary>
        public string AuthnContextDeclRef { get; set; }
    }

    /// <summary>
    /// The comparison methods.
    /// </summary>
    public enum Comparison
    {
        /// <summary>
        /// The resulting authentication context in the authentication
        /// statement MUST be the exact match of at least one of the authentication contexts specified.
        /// </summary>
        Exact,

        /// <summary>
        /// The resulting authentication context in the authentication
        /// statement MUST be at least as strong (as deemed by the responder) as one of the authentication
        /// contexts specified.
        /// </summary>
        Minimum,

        /// <summary>
        /// The resulting authentication context in the authentication
        /// statement MUST be stronger (as deemed by the responder) than any one of the authentication contexts
        /// specified.
        /// </summary>
        Maximum,

        /// <summary>
        /// The resulting authentication context in the authentication
        /// statement MUST be as strong as possible (as deemed by the responder) without exceeding the strength
        /// of at least one of the authentication contexts specified
        /// </summary>
        Better
    }
}
