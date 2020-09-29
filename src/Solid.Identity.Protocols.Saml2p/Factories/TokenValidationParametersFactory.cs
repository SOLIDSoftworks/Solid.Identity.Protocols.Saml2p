using Microsoft.IdentityModel.Tokens;
using Solid.Identity.Protocols.Saml2p.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Factories
{
    public class TokenValidationParametersFactory
    {
        public TokenValidationParameters Create(PartnerSaml2pIdentityProvider partner)
        {
            var parameters = new TokenValidationParameters
            {
                ValidIssuer = partner.Id,
                ValidAudience = partner.ServiceProvider.Id
            };
            parameters.ValidateIssuerSigningKey = parameters.RequireSignedTokens = partner.AssertionSigningKey != null;

            if (parameters.ValidateIssuerSigningKey)
                parameters.IssuerSigningKey = partner.AssertionSigningKey;

            return parameters;
        }
    }
}
