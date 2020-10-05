using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Solid.Identity.Protocols.Saml2p.Abstractions;
using Solid.Identity.Protocols.Saml2p.Options;
using Solid.Identity.Protocols.Saml2p.Models;
using Solid.Identity.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Linq;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.Saml2p.Factories
{
    public class SecurityTokenDescriptorFactory : ISecurityTokenDescriptorFactory
    {
        private IEnumerable<IServiceProviderClaimsProvider> _claimsProviders;
        private ILogger<SecurityTokenDescriptorFactory> _logger;
        private ISystemClock _systemClock;

        public SecurityTokenDescriptorFactory(
            IEnumerable<IServiceProviderClaimsProvider> claimsProviders,
            ILogger<SecurityTokenDescriptorFactory> logger,
            ISystemClock systemClock)
        {
            _claimsProviders = claimsProviders;
            _logger = logger;
            _systemClock = systemClock;
        }

        public async Task<SecurityTokenDescriptor> CreateSecurityTokenDescriptorAsync(ClaimsIdentity identity, PartnerSaml2pServiceProvider partner)
        {
            var instant = identity.FindFirst(ClaimTypes.AuthenticationInstant)?.Value;
            var issuedAt = _systemClock.UtcNow;
            if (instant != null && DateTime.TryParse(instant, out var parsed))
                issuedAt = parsed;

            var now = _systemClock.UtcNow.DateTime;
            var tolerence = partner.MaxClockSkew ?? partner.IdentityProvider.MaxClockSkew ?? TimeSpan.Zero;
            var claims = new List<Claim>();
            foreach (var provider in _claimsProviders.Where(p => p.CanGenerateClaims(partner.Id)))
                claims.AddRange(await provider.GetClaimsAsync(identity, partner));
            
            var attributes = claims
                .Where(c => c.Type != ClaimTypes.NameIdentifier)
                .Where(c => c.Type != ClaimTypes.AuthenticationInstant)
                .Where(c => c.Type != ClaimTypes.AuthenticationMethod)
            ;
            if (!attributes.Any())
                claims.Add(new Claim("http://schemas.solidsoft.works/ws/2020/08/identity/claims/null", bool.TrueString, ClaimValueTypes.Boolean, partner.IdentityProvider.Id));

            var descriptor = new SecurityTokenDescriptor
            {
                Audience = partner.Id,
                Subject = new ClaimsIdentity(claims,"SSO"),
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
