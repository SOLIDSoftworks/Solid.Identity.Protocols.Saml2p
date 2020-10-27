using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Models.Protocol
{
    /// <summary>
    /// The &lt;Status> element of a &lt;SamlResponse>.
    /// </summary>
    public class Status
    {
        /// <summary>
        /// A code representing the status of the activity carried out in response to the corresponding request.
        /// </summary>
        public StatusCode StatusCode { get; set; }
        //public string Message { get; set; }
        //public string Detail { get; set; }
    }
}
