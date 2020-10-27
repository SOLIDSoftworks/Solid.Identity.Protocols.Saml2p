using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Models.Protocol
{
    /// <summary>
    /// The &lt;StatusCode> element specifies a code or a set of nested codes representing the status of the
    /// corresponding request.
    /// </summary>
    public class StatusCode
    {
        /// <summary>
        /// The status code value.
        /// </summary>
        public Uri Value { get; set; }

        /// <summary>
        /// A subordinate status code that provides more specific information on an error condition. 
        /// </summary>
        public StatusCode SubCode { get; set; }
    }
}
