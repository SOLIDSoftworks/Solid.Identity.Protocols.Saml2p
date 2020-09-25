using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Solid.Identity.Protocols.Saml2p.Abstractions.Configuration;
using Solid.Identity.Protocols.Saml2p.Abstractions.Factories;
using Solid.Identity.Protocols.Saml2p.Configuration;
using Solid.Identity.Protocols.Saml2p.Exceptions;
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

        //private TimeSpan GetClockSkewTolerence(IPartnerServiceProvider partner)
        //{
        //    if (partner.ClockSkewTolerence != null) return partner.ClockSkewTolerence.Value;
        //    _logger.LogDebug($"Partner SP {partner.EntityId} doesn't have clock skew tolerence defined. Using default from {partner.IdentityProvider.Issuer}.");
        //    return partner.IdentityProvider.DefaultClockSkewTolerence;
        //}

        private EncryptingCredentials GetEncryptingCredentials(PartnerSaml2pServiceProvider partner)
        {
            if (!partner.EncryptAssertion) return null;
            if (partner.EncryptionKey == null) 
                throw new ArgumentNullException(nameof(partner.EncryptionKey));

            if(string.IsNullOrWhiteSpace(partner.EncryptionKeyWrapAlgorithm))
            {
                if (!(partner.EncryptionKey is SymmetricSecurityKey symmetric))
                    throw new ArgumentException($"{nameof(partner.EncryptionKey)} must be a {nameof(SymmetricSecurityKey)} if no {nameof(partner.EncryptionKeyWrapAlgorithm)} is provided.");
                return new EncryptingCredentials(symmetric, partner.EncryptionAlgorithm);                    
            }

            return new EncryptingCredentials(partner.EncryptionKey, partner.EncryptionKeyWrapAlgorithm, partner.EncryptionAlgorithm);
        }

        private SigningCredentials GetSigningCredentials(PartnerSaml2pServiceProvider partner)
        {
            if (partner.SigningKey == null)
                throw new ArgumentNullException(nameof(partner.SigningKey));
            if (string.IsNullOrWhiteSpace(partner.SigningAlgorithm))
                throw new ArgumentNullException(nameof(partner.SigningAlgorithm));

            if (string.IsNullOrWhiteSpace(partner.SigningDigestAlgorithm))
                return new SigningCredentials(partner.SigningKey, partner.SigningAlgorithm);
            return new SigningCredentials(partner.SigningKey, partner.SigningAlgorithm, partner.SigningDigestAlgorithm);
        }
    }
}
