using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Xml;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml2;
using Solid.Identity.Tokens.Saml2;
using Xunit;
using Xunit.Abstractions;

namespace Solid.Identity.Protocols.Saml2p.Tokens.Saml2.Tests
{
    public class SolidSaml2SecurityTokenHandlerTests
    {
        private ITestOutputHelper _output;

        public SolidSaml2SecurityTokenHandlerTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData("2019-07-10T15:35:50.123Z", "urn:auth_method")]
        public void ShouldIncludeAuthenticationStatement(string authenticationInstant, string authenticationMethod)
        {
            var handler = new SolidSaml2SecurityTokenHandler();
            var identity = CreateIdentity();
            identity.AddClaim(new Claim(ClaimTypes.AuthenticationInstant, authenticationInstant));
            identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, authenticationMethod));
            var descriptor = CreateDesriptor();
            descriptor.Subject = identity;
            var token = handler.CreateToken(descriptor) as Saml2SecurityToken;
            OutputToken(handler, token);

            Assert.NotNull(token);
            var statement = token?.Assertion.Statements.OfType<Saml2AuthenticationStatement>().FirstOrDefault();
            Assert.NotNull(statement);
            Assert.Equal(DateTime.Parse(authenticationInstant), statement.AuthenticationInstant);
            Assert.Equal(new Uri(authenticationMethod), statement.AuthenticationContext.ClassReference);
            Assert.Equal(token.Assertion.Id.Value, statement.SessionIndex);
        }
        [Theory]
        [InlineData("some invalid date", "urn:auth_method")]
        [InlineData("2019-07-10T15:35:50.123Z", "some invalid auth method")]
        [InlineData(null, "urn:auth_method")]
        [InlineData("2019-07-10T15:35:50.123Z", null)]
        [InlineData("some invalid date", "some invalid auth method")]
        [InlineData(null, "some invalid auth method")]
        [InlineData("some invalid date", null)]
        [InlineData(null, null)]
        public void ShouldNotIncludeAuthenticationStatement(string authenticationInstant, string authenticationMethod)
        {
            var handler = new SolidSaml2SecurityTokenHandler();
            var identity = CreateIdentity();
            if(authenticationInstant != null)
                identity.AddClaim(new Claim(ClaimTypes.AuthenticationInstant, authenticationInstant));
            if(authenticationMethod != null)
                identity.AddClaim(new Claim(ClaimTypes.AuthenticationMethod, authenticationMethod));
            var descriptor = CreateDesriptor();
            descriptor.Subject = identity;
            var token = handler.CreateToken(descriptor) as Saml2SecurityToken;
            OutputToken(handler, token);

            Assert.NotNull(token);
            var statement = token?.Assertion.Statements.OfType<Saml2AuthenticationStatement>().FirstOrDefault();
            Assert.Null(statement);
        }

        private void OutputToken(Saml2SecurityTokenHandler handler, Saml2SecurityToken token)
        {
            _output.WriteLine("SAML assertion:");
            if (token == null)
            {
                _output.WriteLine("null");
                return;
            }
            using (var inner = new StringWriter())
            {
                var settings = new XmlWriterSettings
                {
                    OmitXmlDeclaration = true,
                    Indent = true,
                    NewLineOnAttributes = false
                };
                using (var writer = XmlWriter.Create(inner, settings))
                {
                    handler.WriteToken(writer, token);
                }
                _output.WriteLine(inner.ToString());
            }
        }

        private ClaimsIdentity CreateIdentity() => new ClaimsIdentity(new[]
        {
            CreateNameIdentifier("SomeSubject"),
            CreateSamlAttribute(ClaimTypes.Name, "Test user")
        }, "Static", ClaimTypes.Name, ClaimTypes.Role);

        private SecurityTokenDescriptor CreateDesriptor() => new SecurityTokenDescriptor
        {
            Issuer = "https://SomeIssuer",
            IssuedAt = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(5),
            NotBefore = DateTime.UtcNow.AddMinutes(-5),
            Audience = "https://SomeAudience"
        };

        private Claim CreateNameIdentifier(string value)
        {
            var claim = new Claim(ClaimTypes.NameIdentifier, value);
            claim.Properties.Add(ClaimProperties.SamlNameIdentifierFormat, Saml2Constants.NameIdentifierFormats.UnspecifiedString);
            return claim;
        }

        private Claim CreateSamlAttribute(string name, string value)
        {
            var claim = new Claim(name, value);

            return claim;
        }
    }
}
