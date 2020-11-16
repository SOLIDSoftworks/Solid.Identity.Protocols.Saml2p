using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Solid.Identity.Protocols.Saml2p.Abstractions;
using Solid.Identity.Protocols.Saml2p.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Factories
{
    internal class TokenValidationParametersFactory
    {
        private Saml2pOptions _options;

        public TokenValidationParametersFactory(IOptions<Saml2pOptions> options)
        {
            _options = options.Value;
        }

        public TokenValidationParameters Create(ISaml2pIdentityProvider partner)
        {
            var parameters = new TokenValidationParameters
            {
                ValidIssuer = partner.Id,
                ValidAudience = partner.ExpectedIssuer ?? _options.DefaultIssuer
            };
            parameters.ValidateIssuerSigningKey = parameters.RequireSignedTokens = partner.AssertionSigningKeys.Any();

            if (parameters.ValidateIssuerSigningKey)
                parameters.IssuerSigningKeys = partner.AssertionSigningKeys;

            parameters.TokenDecryptionKeys = partner.AssertionDecryptionKeys;

            return parameters;
        }
    }
}
