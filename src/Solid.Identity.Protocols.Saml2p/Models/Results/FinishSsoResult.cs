using Microsoft.IdentityModel.Tokens.Saml2;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;

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
        /// <param name="partnerId">The id of the Saml2p SSO partner.</param>
        /// <param name="token">The XML representation of the received Saml2 token.</param>
        /// <param name="securityToken">The <see cref="Saml2SecurityToken"/> from a <see cref="SamlResponse"/>.</param>
        /// <param name="subject">The <see cref="ClaimsPrincipal"/> that was created from <paramref name="securityToken"/>.</param>
        /// <param name="properties">The <see cref="AuthenticationProperties"/> instance used when signing in.</param>
        /// <returns>A success result.</returns>
        public static FinishSsoResult Success(string partnerId, string token, Saml2SecurityToken securityToken, ClaimsPrincipal subject, AuthenticationProperties properties)
        {
            return new FinishSsoResult
            {
                Status = Saml2pConstants.Statuses.Success,
                PartnerId = partnerId,
                Token = token,
                SecurityToken = securityToken,
                Subject = subject,
                Properties = properties
            };
        }

        ///// <summary>
        ///// Creates a failure result.
        ///// </summary>
        ///// <returns>A failure result.</returns>
        //public static FinishSsoResult Fail()
        //{
        //    return new FinishSsoResult
        //    {
        //    };
        //}

        /// <summary>
        /// Creates a failure result with a status.
        /// </summary>
        /// <param name="partnerId">The id of the Saml2p SSO partner.</param>
        /// <param name="status">The status of the SSO result.</param>
        /// <param name="subStatus">The substatus of the SSO result.</param>
        /// <returns>A failure result.</returns>
        public static FinishSsoResult Fail(string partnerId, Uri status, Uri subStatus)
        {
            return new FinishSsoResult
            {
                PartnerId = partnerId,
                Status = status,
                SubStatus = subStatus
            };
        }

        /// <summary>
        /// Indicates whether the result is successful.
        /// </summary>
        public bool IsSuccessful => Status == Saml2pConstants.Statuses.Success;

        /// <summary>
        /// The <see cref="Saml2SecurityToken"/> from a <see cref="SamlResponse"/>.
        /// </summary>
        public Saml2SecurityToken SecurityToken { get; private set; }
        
        /// <summary>
        /// The XML representation of the received Saml2 token.
        /// </summary>
        public string Token { get; private set; }

        /// <summary>
        /// The <see cref="ClaimsPrincipal"/> that was created from <see cref="SecurityToken"/>.
        /// </summary>
        public ClaimsPrincipal Subject { get; private set; }
        
        /// <summary>
        /// The <see cref="AuthenticationProperties"/> instance used when signing in.
        /// </summary>
        public AuthenticationProperties Properties { get; private set; }

        /// <summary>
        /// The status of the SSO response.
        /// </summary>
        public Uri Status { get; private set; }

        /// <summary>
        /// The substatus of the SSO response.
        /// </summary>
        public Uri SubStatus { get; private set; }

        /// <summary>
        /// The id of the Saml2p SSO partner.
        /// </summary>
        public string PartnerId { get; private set; }
    }
}
