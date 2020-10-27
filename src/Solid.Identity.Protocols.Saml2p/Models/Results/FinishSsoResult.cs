using Microsoft.IdentityModel.Tokens.Saml2;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Models.Results
{
    /// <summary>
    /// A result of finishing Saml2p SSO.
    /// </summary>
    public class FinishSsoResult
    {
        private FinishSsoResult() { }

        /// <summary>
        /// Creates a success result.
        /// </summary>
        /// <param name="token">The <see cref="Saml2SecurityToken"/> from a <see cref="SamlResponse"/>.</param>
        /// <param name="subject">The <see cref="ClaimsPrincipal"/> that was created from <paramref name="token"/>.</param>
        /// <returns>A success result.</returns>
        public static FinishSsoResult Success(Saml2SecurityToken token, ClaimsPrincipal subject)
        {
            return new FinishSsoResult
            {
                SecurityToken = token,
                Subject = subject
            };
        }

        /// <summary>
        /// Creates a failure result.
        /// </summary>
        /// <returns>A failure result.</returns>
        public static FinishSsoResult Fail()
        {
            return new FinishSsoResult
            {
            };
        }

        /// <summary>
        /// Indicates whether the result is successful.
        /// </summary>
        public bool IsSuccessful => SecurityToken != null && Subject != null;

        /// <summary>
        /// The <see cref="Saml2SecurityToken"/> from a <see cref="SamlResponse"/>.
        /// </summary>
        public Saml2SecurityToken SecurityToken { get; private set; }

        /// <summary>
        /// The <see cref="ClaimsPrincipal"/> that was created from <see cref="SecurityToken"/>.
        /// </summary>
        public ClaimsPrincipal Subject { get; private set; }
    }
}
