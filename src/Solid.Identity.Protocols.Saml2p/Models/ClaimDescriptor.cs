using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Models
{
    /// <summary>
    /// A descriptor for a type of <see cref="Claim"/>.
    /// </summary>
    public class ClaimDescriptor
    {
        /// <summary>
        /// Creates a <see cref="ClaimDescriptor"/> instance.
        /// </summary>
        /// <param name="type">The claim type.</param>
        /// <param name="description">A description of the claim type.</param>
        /// <param name="valueType">The value type of the claim type.</param>
        public ClaimDescriptor(string type, string description = null, string valueType = null)
        {
            Type = type;
            Description = description;
            ValueType = valueType;
        }

        /// <summary>
        /// The claim type.
        /// </summary>
        public string Type { get; }

        /// <summary>
        /// The value type of the claim type.
        /// </summary>
        public string ValueType { get; }

        /// <summary>
        /// A description of the claim type.
        /// </summary>
        public string Description { get; }
    }
}
