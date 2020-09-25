using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Models.Protocol
{
    public class NameIdPolicy
    {
        public bool? AllowCreate { get; set; }
        public string Format { get; set; }
    }
}
