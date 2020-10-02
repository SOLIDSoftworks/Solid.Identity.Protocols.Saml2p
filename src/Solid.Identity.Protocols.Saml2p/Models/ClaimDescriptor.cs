using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Models
{
    public class ClaimDescriptor
    {
        public ClaimDescriptor(string type, string description = null, string valueType = null)
        {
            Type = type;
            Description = description;
            ValueType = valueType;
        }

        public string Type { get; }
        public string ValueType { get; }
        public string Description { get; }
    }
}
