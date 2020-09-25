using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Models.Protocol
{
    public class StatusCode
    {
        public Uri Value { get; set; }
        public StatusCode SubCode { get; set; }
    }
}
