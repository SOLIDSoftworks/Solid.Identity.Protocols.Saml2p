using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Solid.Identity.Protocols.Saml2p.Abstractions.Configuration;
using Solid.Identity.Protocols.Saml2p.Abstractions.Factories;
using Solid.Identity.Protocols.Saml2p.Configuration;
using Solid.Identity.Protocols.Saml2p.Models;
using Solid.Identity.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Factories
{
    public class SecurityTokenDescriptorFactory : ISecurityTokenDescriptorFactory
    {
        private ILogger<SecurityTokenDescriptorFactory> _logger;
        private ISystemClock _systemClock;

        public SecurityTokenDescriptorFactory(
            ILogger<SecurityTokenDescriptorFactory> logger,
            ISystemClock systemClock)
        {
            _logger = logger;
            _systemClock = systemClock;
        }

        public SecurityTokenDescriptor CreateSecurityTokenDescriptor(ClaimsIdentity identity, PartnerSaml2pServiceProvider partner)
        {
            var instant = identity.FindFirst(ClaimTypes.AuthenticationInstant)?.Value;
            var issuedAt = _systemClock.UtcNow;
            if (instant != null && DateTime.TryParse(instant, out var parsed))
                issuedAt = parsed;

            var now = _systemClock.UtcNow.DateTime;
            var tolerence = partner.MaxClockSkew;

            var descriptor = new SecurityTokenDescriptor
            {
                Audience = partner.Id,
                Subject = identity,
                Issuer = partner.IdentityProvider.Id,
                IssuedAt = issuedAt.DateTime,
                NotBefore = now.Subtract(tolerence),
                Expires = now.Add(tolerence),
                SigningCredentials = GetSigningCredentials(partner),
                EncryptingCredentials = GetEncryptingCredentials(partner)
            };

            return descriptor;
        }

        private EncryptingCredentials GetEncryptingCredentials(PartnerSaml2pServiceProvider partner)
        {
            if (!partner.EncryptAssertion) return null;
            if (partner.AssertionEncryptionKey == null) 
                throw new ArgumentNullException(nameof(partner.AssertionEncryptionKey));

            if(string.IsNullOrWhiteSpace(partner.AssertionEncryptionKeyWrapAlgorithm))
            {
                if (!(partner.AssertionEncryptionKey is SymmetricSecurityKey symmetric))
                    throw new ArgumentException($"{nameof(partner.AssertionEncryptionKey)} must be a {nameof(SymmetricSecurityKey)} if no {nameof(partner.AssertionEncryptionKeyWrapAlgorithm)} is provided.");
                return new EncryptingCredentials(symmetric, partner.AssertionEncryptionAlgorithm);                    
            }

            return new EncryptingCredentials(partner.AssertionEncryptionKey, partner.AssertionEncryptionKeyWrapAlgorithm, partner.AssertionEncryptionAlgorithm);
        }

        private SigningCredentials GetSigningCredentials(PartnerSaml2pServiceProvider partner)
        {
            if (partner.AssertionSigningKey == null)
                throw new ArgumentNullException(nameof(partner.AssertionSigningKey));
            if (string.IsNullOrWhiteSpace(partner.AssertionSigningAlgorithm))
                throw new ArgumentNullException(nameof(partner.AssertionSigningAlgorithm));

            if (string.IsNullOrWhiteSpace(partner.AssertionSigningDigestAlgorithm))
                return new SigningCredentials(partner.AssertionSigningKey, partner.AssertionSigningAlgorithm);
            return new SigningCredentials(partner.AssertionSigningKey, partner.AssertionSigningAlgorithm, partner.AssertionSigningDigestAlgorithm);
        }
    }
}
