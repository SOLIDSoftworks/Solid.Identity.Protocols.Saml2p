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
using System.Xml;
using Microsoft.IdentityModel.Tokens.Saml2;
using Microsoft.Extensions.Options;
using Solid.Identity.Protocols.Saml2p.Logging;

namespace Solid.Identity.Protocols.Saml2p.Factories
{
    public class SecurityTokenDescriptorFactory : ISecurityTokenDescriptorFactory
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
                if (!await provider.CanGenerateClaimsAsync(partner))
                {
                    _logger.LogInformation($"Generating claims using {provider.GetType().Name}.");
                    var generated = await provider.GenerateClaimsAsync(identity, partner, issuer);
                    Trace($"Generated claims from {provider.GetType().Name}.", generated);
                    claims.AddRange(generated);
                }
            }

            if (!claims.Any(c => c.Type == ClaimTypes.AuthenticationInstant))
                claims.Add(new Claim(ClaimTypes.AuthenticationInstant, XmlConvert.ToString(issuedAt, "yyyy-MM-ddTHH:mm:ss.fffZ"), ClaimValueTypes.DateTime));
            if (!claims.Any(c => c.Type == ClaimTypes.AuthenticationMethod))
                claims.Add(new Claim(ClaimTypes.AuthenticationMethod, Saml2pConstants.Classes.UnspecifiedString));

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
                Subject = new ClaimsIdentity(claims,"SSO"),
                Issuer = issuer,
                IssuedAt = issuedAt,
                NotBefore = issuedAt.Subtract(tolerence),
                Expires = expires,
                SigningCredentials = GetSigningCredentials(partner),
                //EncryptingCredentials = GetEncryptingCredentials(partner)
            };

            return descriptor;
        }

        //private EncryptingCredentials GetEncryptingCredentials(ISaml2pServiceProvider partner)
        //{
        //    if (!partner.RequiresEncryptedAssertion) return null;
        //    if (partner.AssertionEncryptionKey == null) 
        //        throw new ArgumentNullException(nameof(partner.AssertionEncryptionKey));

        //    if(string.IsNullOrWhiteSpace(partner.AssertionEncryptionKeyWrapAlgorithm))
        //    {
        //        if (!(partner.AssertionEncryptionKey is SymmetricSecurityKey symmetric))
        //            throw new ArgumentException($"{nameof(partner.AssertionEncryptionKey)} must be a {nameof(SymmetricSecurityKey)} if no {nameof(partner.AssertionEncryptionKeyWrapAlgorithm)} is provided.");
        //        return new EncryptingCredentials(symmetric, partner.AssertionEncryptionAlgorithm);
        //    }

        //    return new EncryptingCredentials(partner.AssertionEncryptionKey, partner.AssertionEncryptionKeyWrapAlgorithm, partner.AssertionEncryptionAlgorithm);
        //}

        private SigningCredentials GetSigningCredentials(ISaml2pServiceProvider partner)
        {
            if (partner.AssertionSigningKey == null)
                throw new ArgumentNullException(nameof(partner.AssertionSigningKey));
            if (string.IsNullOrWhiteSpace(partner.AssertionSigningAlgorithm))
                throw new ArgumentNullException(nameof(partner.AssertionSigningAlgorithm));

            var credentials = null as SigningCredentials;
            if (string.IsNullOrWhiteSpace(partner.AssertionSigningDigestAlgorithm))
                credentials = new SigningCredentials(partner.AssertionSigningKey, partner.AssertionSigningAlgorithm);
            else
                credentials = new SigningCredentials(partner.AssertionSigningKey, partner.AssertionSigningAlgorithm, partner.AssertionSigningDigestAlgorithm);

            Trace("Signing credentials created.", credentials);
            return credentials;
        }

        private void Trace(string prefix, SigningCredentials credentials)
        {
            if (!_logger.IsEnabled(LogLevel.Trace)) return;
            var state = new Dictionary<string, string>
            {
                { "securityKeyName", (credentials.Key is X509SecurityKey x509) ? x509.Certificate.Subject : credentials.Key.KeyId },
                { "securityKeyType", credentials.Key.KeyId },
                { "algorithm", credentials.Algorithm },
                { "digest", credentials.Digest }
            };

            _logger.LogTrace(prefix + Environment.NewLine + "{state}", new WrappedLogMessageState(state));
        }

        private void Trace(string prefix, IEnumerable<Claim> generated)
        {
            if (!_logger.IsEnabled(LogLevel.Trace)) return;
            _logger.LogTrace(prefix + Environment.NewLine + "{state}", new WrappedLogMessageState(generated.Select(c => $"{c.Type}: {c.Value}")));
        }
    }
}
