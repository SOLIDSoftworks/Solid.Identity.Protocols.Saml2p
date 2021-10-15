using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Solid.Identity.Protocols.Saml2p.Abstractions;
using Solid.Identity.Protocols.Saml2p.Options;
using Solid.Identity.Protocols.Saml2p.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Microsoft.IdentityModel.Tokens.Saml2;
using Microsoft.Extensions.Options;
using Solid.Identity.Protocols.Saml2p.Logging;
using System.Security;
using Solid.Identity.Protocols.Saml2p.Providers;

namespace Solid.Identity.Protocols.Saml2p.Factories
{
    internal class SecurityTokenDescriptorFactory : ISecurityTokenDescriptorFactory
    {
        private Saml2pOptions _options;
        private IEnumerable<IServiceProviderClaimsProvider> _claimsProviders;
        private ILogger<SecurityTokenDescriptorFactory> _logger;
        private ISystemClock _systemClock;

        public SecurityTokenDescriptorFactory(
            IOptions<Saml2pOptions> options,
            IEnumerable<IServiceProviderClaimsProvider> claimsProviders,
            ILogger<SecurityTokenDescriptorFactory> logger,
            ISystemClock systemClock)
        {
            _options = options.Value;
            _claimsProviders = claimsProviders;
            _logger = logger;
            _systemClock = systemClock;
        }

        public async ValueTask<SecurityTokenDescriptor> CreateSecurityTokenDescriptorAsync(ClaimsIdentity identity, ISaml2pServiceProvider partner)
        {
            var instant = identity.FindFirst(ClaimTypes.AuthenticationInstant)?.Value;
            var issuedAt = _systemClock.UtcNow.DateTime;
            if (instant != null && DateTime.TryParse(instant, out var parsed))
                issuedAt = parsed;

            var issuer = partner.ExpectedIssuer ?? _options.DefaultIssuer;

            var lifetime = partner.TokenLifeTime ?? _options.DefaultTokenLifetime;
            var tolerence = partner.MaxClockSkew ?? _options.DefaultMaxClockSkew ?? TimeSpan.Zero;
            var claims = new List<Claim>();
            foreach (var provider in _claimsProviders)
            {
                if (await provider.CanGenerateClaimsAsync(partner))
                {
                    _logger.LogInformation($"Generating claims using {provider.GetType().Name}.");
                    var generated = await provider.GenerateClaimsAsync(identity, partner, issuer);
                    Trace($"Generated claims from {provider.GetType().Name}.", generated);
                    claims.AddRange(generated);
                }
            }

            var defaults = new[] { ClaimTypes.NameIdentifier, ClaimTypes.AuthenticationInstant, ClaimTypes.AuthenticationMethod };
            var required = (partner.RequiredClaims ?? Enumerable.Empty<string>()).Distinct();
            var optional = (partner.OptionalClaims ?? Enumerable.Empty<string>()).Distinct();
            var supported = defaults.Concat(required).Concat(optional).Distinct();

            Debug("Checking for all required claims. Required claim types:", required);
            if (required.Except(claims.Select(c => c.Type)).Any())
                throw new SecurityException("Unable to generate all required claims.");

            Debug("Filtering generated claims. Supported claim types:", supported);
            claims = claims.Where(c => supported.Contains(c.Type)).ToList();
            Trace($"Filtered claims.", claims);

            AddRequiredClaims(identity, claims, issuedAt, issuer);

            var attributes = claims
                .Where(c => c.Type != ClaimTypes.NameIdentifier)
                .Where(c => c.Type != ClaimTypes.AuthenticationInstant)
                .Where(c => c.Type != ClaimTypes.AuthenticationMethod)
            ;

            if (!attributes.Any())
                claims.Add(new Claim("http://schemas.solidsoft.works/ws/2020/08/identity/claims/null", bool.TrueString, ClaimValueTypes.Boolean, issuer));

            foreach(var attribute in attributes)
            {
                if (attribute.Properties.ContainsKey(ClaimProperties.SamlAttributeNameFormat)) continue;
                attribute.Properties.Add(ClaimProperties.SamlAttributeNameFormat, "urn:oasis:names:tc:SAML:2.0:attrname-format:unspecified");
            }

            var expires = issuedAt
                .Add(lifetime)
                .Add(tolerence)
            ;

            var descriptor = new SecurityTokenDescriptor
            {
                Audience = partner.Id,
                Subject = new ClaimsIdentity(claims, "SSO"),
                Issuer = issuer,
                IssuedAt = issuedAt,
                NotBefore = issuedAt.Subtract(tolerence),
                Expires = expires,
                SigningCredentials = GetSigningCredentials(partner),
                EncryptingCredentials = GetEncryptingCredentials(partner)
            };

            return descriptor;
        }

        private void AddRequiredClaims(ClaimsIdentity identity, List<Claim> claims, DateTime issuedAt, string issuer)
        {
            if (!claims.Any(c => c.Type == ClaimTypes.NameIdentifier))
            {
                if (identity.TryFindFirst(ClaimTypes.NameIdentifier, out var nameId))
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, nameId.Value, null, issuer));
            }

            if (!claims.Any(c => c.Type == ClaimTypes.AuthenticationInstant))
            {
                claims.Add(new Claim(ClaimTypes.AuthenticationInstant, XmlConvert.ToString(issuedAt, "yyyy-MM-ddTHH:mm:ss.fffZ"), ClaimValueTypes.DateTime, null, issuer));
            }

            if (!claims.Any(c => c.Type == ClaimTypes.AuthenticationMethod))
            {
                if (identity.TryFindFirst(ClaimTypes.AuthenticationMethod, out var authenticationMethod))
                    claims.Add(new Claim(ClaimTypes.AuthenticationMethod, authenticationMethod.Value, null, issuer));
                else
                    claims.Add(new Claim(ClaimTypes.AuthenticationMethod, Saml2pConstants.Classes.UnspecifiedString, null, issuer));
            }
        }

        private EncryptingCredentials GetEncryptingCredentials(ISaml2pServiceProvider partner)
        {
            if (!partner.RequiresEncryptedAssertion) return null;

            if (partner.AssertionEncryptionKey == null)
                throw new ArgumentNullException(nameof(partner.AssertionEncryptionKey));

            if (partner.AssertionEncryptionMethod == null)
                throw new ArgumentNullException(nameof(partner.AssertionEncryptionMethod));

            var credentials = partner.AssertionEncryptionMethod.CreateCredentials(partner.AssertionEncryptionKey);
            Trace("Encrypting credentials created.", credentials);
            return credentials;
        }

        private SigningCredentials GetSigningCredentials(ISaml2pServiceProvider partner)
        {
            if (partner.AssertionSigningKey == null)
                throw new ArgumentNullException(nameof(partner.AssertionSigningKey));
            if(partner.AssertionSigningMethod == null)
                throw new ArgumentNullException(nameof(partner.AssertionSigningMethod));

            var credentials = partner.AssertionSigningMethod.CreateCredentials(partner.AssertionSigningKey);
            Trace("Signing credentials created.", credentials);
            return credentials;
        }

        private void Debug(string prefix, object obj)
        {
            if (!_logger.IsEnabled(LogLevel.Debug)) return;
            _logger.LogDebug(prefix + Environment.NewLine + "{state}", new WrappedLogMessageState(obj));
        }

        private void Trace(string prefix, object obj)
        {
            if (!_logger.IsEnabled(LogLevel.Trace)) return;
            _logger.LogTrace(prefix + Environment.NewLine + "{state}", new WrappedLogMessageState(obj));
        }
    }
}
