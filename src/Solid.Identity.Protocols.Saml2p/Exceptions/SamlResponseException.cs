using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Exceptions
{
    /// <summary>
    /// A exception that can be thrown during SP authentication.
    /// </summary>
    public class SamlResponseException : Exception
    {
        /// <summary>
        /// Creates a new exception.
        /// </summary>
        /// <param name="partnerId">The id of the partner to send the response.</param>
        /// <param name="status">The status of the response.</param>
        /// <param name="subStatus">The substatus of the response.</param>
        public SamlResponseException(string partnerId, Uri status, Uri subStatus) 
            : base(GenerateMessage(partnerId, status, subStatus))
        {
        }

        private static string GenerateMessage(string partnerId, Uri status, Uri subStatus)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"SSO using parnter '{partnerId}' failed.");
            builder.AppendLine($"  SAMLResponse status:    {status}");
            builder.AppendLine($"  SAMLResponse substatus: {subStatus}");
            return builder.ToString();
        }
    }
}
