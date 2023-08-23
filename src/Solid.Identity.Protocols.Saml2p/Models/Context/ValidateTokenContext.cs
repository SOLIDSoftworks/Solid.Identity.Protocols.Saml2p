using Solid.Identity.Protocols.Saml2p.Options;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml2;
using Solid.Identity.Protocols.Saml2p.Abstractions;

namespace Solid.Identity.Protocols.Saml2p.Models.Context
{
    /// <summary>
    /// A context class used with <see cref="Saml2pServiceProviderEvents.OnValidatingToken"/> and <see cref="Saml2pServiceProviderEvents.OnValidatedToken"/>.
    /// </summary>
    public class ValidateTokenContext
    {
        /// <summary>
        /// The partner id.
        /// </summary>
        public string PartnerId { get; internal set; }

        /// <summary>
        /// The Saml2p IDP partner.
        /// </summary>
        public ISaml2pIdentityProvider Partner { get; internal set; }

        /// <summary>
        /// The original <see cref="AuthnRequest"/> that was sent to the IDP.
        /// </summary>
        public AuthnRequest Request { get; internal set; }

        /// <summary>
        /// The <see cref="SamlResponse"/> received from the IDP.
        /// </summary>
        public SamlResponse Response { get; internal set; }

        /// <summary>
        /// The <see cref="Microsoft.IdentityModel.Tokens.TokenValidationParameters"/> used to validate the incoming security token.
        /// </summary>
        public TokenValidationParameters TokenValidationParameters { get; internal set; }

        /// <summary>
        /// The <see cref="Saml2SecurityTokenHandler"/> used to validate the incoming security token.
        /// </summary>
        public Saml2SecurityTokenHandler Handler { get; internal set; }

        /// <summary>
        /// The <see cref="ClaimsPrincipal"/> that is the result of the security token validation.
        /// </summary>
        public ClaimsPrincipal Subject { get; set; }

        /// <summary>
        /// The <see cref="Saml2SecurityToken"/> that was parsed from the incoming security token and subsequently used to create <see cref="Subject"/>.
        /// </summary>
        public Saml2SecurityToken SecurityToken { get; set; }
    }
}
