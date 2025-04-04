using Microsoft.IdentityModel.Tokens.Saml2;
using Moq;
using Solid.Identity.Protocols.Saml2p.Abstractions;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using Solid.Identity.Protocols.Saml2p.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Xunit;
using FluentAssertions;
using FluentAssertions.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;

namespace Solid.Identity.Protocols.Saml2p.Tests
{
    public class Saml2pSerializerTests
    {
        private static readonly string _assertionNamespace = "urn:oasis:names:tc:SAML:2.0:assertion";
        private static readonly string _protocolNamespace = "urn:oasis:names:tc:SAML:2.0:protocol";

        private Saml2pSerializer _serializer;
        private Mock<Saml2SecurityTokenHandler> _mockHandler;

        public Saml2pSerializerTests()
        {
            IdentityModelEventSource.ShowPII = true;
            
            _mockHandler = new Mock<Saml2SecurityTokenHandler>();
            _mockHandler.CallBase = true;
            var mockWriterFactory = new Mock<IXmlWriterFactory>();
            mockWriterFactory.Setup(f => f.CreateXmlWriter(It.IsAny<TextWriter>())).Returns<TextWriter>(w => XmlWriter.Create(w));
            var mockReaderFactory = new Mock<IXmlReaderFactory>();
            mockReaderFactory.Setup(f => f.CreateXmlReader(It.IsAny<TextReader>())).Returns<TextReader>(r => XmlReader.Create(r));
            var mockLogger = new Mock<ILogger>();
            _serializer = new Saml2pSerializer(_mockHandler.Object, mockReaderFactory.Object, mockWriterFactory.Object, mockLogger.Object);
        }

        [Fact]
        public void ShouldWriteSamlResponseElementWithDefaultVersion()
        {
            var response = new SamlResponse();

            var serialized = _serializer.SerializeSamlResponse(response);
            Assert.NotNull(serialized);

            var doc = XDocument.Parse(serialized);
            var root = doc.Root;
            var version = root.Attribute("Version");
            Assert.Equal("samlp", doc.Root.GetPrefixOfNamespace(root.Name.Namespace));
            Assert.Equal("Response", root.Name.LocalName);
            Assert.Equal(_protocolNamespace, root.Name.Namespace);
            Assert.NotNull(version?.Value);
            Assert.Equal("2.0", version.Value);
        }

        [Fact]
        public void ShouldWriteSamlResponseVersionAttribute()
        {
            var expected = $"{Guid.NewGuid()}";
            var response = new SamlResponse
            {
                Version = expected
            };

            var serialized = _serializer.SerializeSamlResponse(response);
            Assert.NotNull(serialized);

            var doc = XDocument.Parse(serialized);
            var root = doc.Root;
            var attribute = root.Attribute("Version");
            Assert.NotNull(attribute?.Value);
            Assert.Equal(expected, attribute.Value);
        }

        [Fact]
        public void ShouldWriteSamlResponseIdAttribute()
        {
            var expected = $"_{Guid.NewGuid()}";
            var response = new SamlResponse
            {
                Id = expected
            };

            var serialized = _serializer.SerializeSamlResponse(response);
            Assert.NotNull(serialized);

            var doc = XDocument.Parse(serialized);
            var root = doc.Root;
            var attribute = root.Attribute("ID");
            Assert.NotNull(attribute?.Value);
            Assert.Equal(expected, attribute.Value);
        }

        [Fact]
        public void ShouldWriteSamlResponseIssueInstantAttribute()
        {
            var now = DateTime.UtcNow;
            var expected = now.ToString("o");
            var response = new SamlResponse
            {
                IssueInstant = now
            };

            var serialized = _serializer.SerializeSamlResponse(response);
            Assert.NotNull(serialized);

            var doc = XDocument.Parse(serialized);
            var root = doc.Root;
            var attribute = root.Attribute("IssueInstant");
            Assert.NotNull(attribute?.Value);
            Assert.Equal(expected, attribute.Value);
        }

        [Theory]
        [InlineData("https://notused")]
        public void ShouldWriteSamlResponseDestinationAttribute(string expected)
        {
            var url = new Uri(expected);
            var response = new SamlResponse
            {
                Destination = url
            };

            var serialized = _serializer.SerializeSamlResponse(response);
            Assert.NotNull(serialized);

            var doc = XDocument.Parse(serialized);
            var root = doc.Root;
            var attribute = root.Attribute("Destination");
            Assert.NotNull(attribute?.Value);
            Assert.Equal(expected, attribute.Value);
        }

        [Fact]
        public void ShouldWriteSamlResponseInResponseToAttribute()
        {
            var expected = $"_{Guid.NewGuid()}";
            var response = new SamlResponse
            {
                InResponseTo = expected
            };

            var serialized = _serializer.SerializeSamlResponse(response);
            Assert.NotNull(serialized);

            var doc = XDocument.Parse(serialized);
            var root = doc.Root;
            var attribute = root.Attribute("InResponseTo");
            Assert.NotNull(attribute?.Value);
            Assert.Equal(expected, attribute.Value);
        }

        [Fact]
        public void ShouldWriteSamlResponseIssuerElement()
        {
            var expected = $"uri:{Guid.NewGuid()}";
            var response = new SamlResponse
            {
                Issuer = expected
            };

            var serialized = _serializer.SerializeSamlResponse(response);
            Assert.NotNull(serialized);

            var doc = XDocument.Parse(serialized);
            var root = doc.Root;
            var issuer = root.Element(XName.Get("Issuer", _assertionNamespace));
            Assert.NotNull(issuer);
            Assert.Equal("saml", issuer.GetPrefixOfNamespace(issuer.Name.NamespaceName));
            Assert.Equal(expected, issuer.Value);
        }

        [Fact]
        public void ShouldWriteSamlResponseStatusElement()
        {
            var response = new SamlResponse
            {
                Status = new Status()
            };

            var serialized = _serializer.SerializeSamlResponse(response);
            Assert.NotNull(serialized);

            var doc = XDocument.Parse(serialized);
            var root = doc.Root;
            var status = root.Element(XName.Get("Status", _protocolNamespace));
            Assert.NotNull(status);
        }

        [Theory]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:Success")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:Requester")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:Responder")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:VersionMismatch")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:AuthnFailed")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:InvalidAttrNameOrValue")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:InvalidNameIDPolicy")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:NoAuthnContext")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:NoAvailableIDP")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:NoPassive")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:NoSupportedIDP")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:PartialLogout")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:ProxyCountExceeded")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:RequestDenied")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:RequestUnsupported")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:RequestVersionDeprecated")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:RequestVersionTooHigh")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:RequestVersionTooLow")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:ResourceNotRecognized")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:TooManyResponses")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:UnknownAttrProfile")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:UnknownPrincipal")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:UnsupportedBinding")]
        public void ShouldWriteSamlResponseStatusCodeElement(string expected)
        {
            var uri = new Uri(expected);
            var response = new SamlResponse
            {
                Status = new Status
                {
                    StatusCode = new StatusCode
                    {
                        Value = uri
                    }
                }
            };

            var serialized = _serializer.SerializeSamlResponse(response);
            Assert.NotNull(serialized);

            var doc = XDocument.Parse(serialized);
            var root = doc.Root;
            var status = root.Element(XName.Get("Status", _protocolNamespace));
            Assert.NotNull(status);
            var code = status.Element(XName.Get("StatusCode", _protocolNamespace));
            Assert.NotNull(code);
            var attribute = code.Attribute("Value");
            Assert.NotNull(attribute?.Value);
            Assert.Equal(expected, attribute.Value);
        }

        [Fact]
        public void ShouldWriteSamlResponseAssertionElement()
        {
            var issuer = new Saml2NameIdentifier("issuer");
            var assertion = new Saml2Assertion(issuer)
            {
                Subject = new Saml2Subject(new Saml2NameIdentifier("subject"))
            };

            var token = new Saml2SecurityToken(assertion);

            var response = new SamlResponse
            {
                SecurityToken = token
            };

            var serialized = _serializer.SerializeSamlResponse(response);
            Assert.NotNull(serialized);

            var doc = XDocument.Parse(serialized);
            var root = doc.Root;
            _mockHandler.Verify(h => h.WriteToken(It.IsAny<XmlWriter>(), token), Times.Once());
            var element = root.Element(XName.Get("Assertion", _assertionNamespace));
            Assert.NotNull(element);
        }

        [Fact]
        public void ShouldIgnoreInvalidResponse()
        {
            var invalid = $"invalid";

            var request = _serializer.DeserializeSamlResponse(invalid);
            Assert.Null(request);
        }

        [Fact]
        public void ShouldReadSamlResponseElement()
        {
            var xml = $"<samlp:Response xmlns:samlp=\"{_protocolNamespace}\" Version=\"2.0\"></samlp:Response>";

            var request = _serializer.DeserializeSamlResponse(xml);
            Assert.NotNull(request);
        }
        

        [Fact]
        public void ShouldReadSamlResponseSignature()
        {
            using var key = RSA.Create(2048);
            var xml = $"<samlp:Response xmlns:samlp=\"{_protocolNamespace}\" Version=\"2.0\"></samlp:Response>";
            var document = new XmlDocument();
            document.LoadXml(xml);
            document.SignXml(key);

            var signed = document.OuterXml;

            var request = _serializer.DeserializeSamlResponse(signed);
            Assert.NotNull(request.Signature);
        }

        [Fact]
        public void ShouldReadSamlResponseVersionAttribute()
        {
            var expected = $"{Guid.NewGuid()}";
            var xml = $"<samlp:Response xmlns:samlp=\"{_protocolNamespace}\" Version=\"{expected}\"></samlp:Response>";

            var response = _serializer.DeserializeSamlResponse(xml);
            Assert.NotNull(response?.Version);
            Assert.Equal(expected, response.Version);
        }

        [Fact]
        public void ShouldReadSamlResponseIdAttribute()
        {
            var expected = $"{Guid.NewGuid()}";
            var xml = $"<samlp:Response xmlns:samlp=\"{_protocolNamespace}\" ID=\"{expected}\"></samlp:Response>";

            var response = _serializer.DeserializeSamlResponse(xml);
            Assert.NotNull(response?.Id);
            Assert.Equal(expected, response.Id);
        }

        [Fact]
        public void ShouldReadSamlResponseIssueInstantAttribute()
        {
            var expected = DateTime.UtcNow;
            var value = expected.ToString("o");
            var xml = $"<samlp:Response xmlns:samlp=\"{_protocolNamespace}\" IssueInstant=\"{value}\"></samlp:Response>";

            var response = _serializer.DeserializeSamlResponse(xml);
            Assert.NotNull(response?.IssueInstant);
            Assert.Equal(expected, response.IssueInstant);
        }

        [Theory]
        [InlineData("https://notused")]
        public void ShouldReadSamlResponseDestinationAttribute(string url)
        {
            var expected = new Uri(url);
            var xml = $"<samlp:Response xmlns:samlp=\"{_protocolNamespace}\" Destination=\"{expected}\"></samlp:Response>";

            var response = _serializer.DeserializeSamlResponse(xml);
            Assert.NotNull(response?.Destination);
            Assert.Equal(expected, response.Destination);
        }

        [Fact]
        public void ShouldReadSamlResponseInResponseToAttribute()
        {
            var expected = $"_{Guid.NewGuid()}";
            var xml = $"<samlp:Response xmlns:samlp=\"{_protocolNamespace}\" InResponseTo=\"{expected}\"></samlp:Response>";

            var response = _serializer.DeserializeSamlResponse(xml);
            Assert.NotNull(response?.InResponseTo);
            Assert.Equal(expected, response.InResponseTo);
        }

        [Fact]
        public void ShouldReadSamlResponseIssuerElement()
        {
            var expected = $"uri:{Guid.NewGuid()}";
            var xml = $@"
                    <samlp:Response xmlns:samlp=""{_protocolNamespace}"" xmlns:saml=""{_assertionNamespace}"">
                        <saml:Issuer>{expected}</saml:Issuer>
                    </samlp:Response>";

            var response = _serializer.DeserializeSamlResponse(xml);
            Assert.NotNull(response?.Issuer);
            Assert.Equal(expected, response.Issuer);
        }

        [Fact]
        public void ShouldReadSamlResponseStatusElement()
        {
            var xml = $@"
                    <samlp:Response xmlns:samlp=""{_protocolNamespace}"">
                        <samlp:Status></samlp:Status>
                    </samlp:Response>";

            var response = _serializer.DeserializeSamlResponse(xml);
            Assert.NotNull(response?.Status);
        }

        [Theory]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:Success")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:Requester")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:Responder")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:VersionMismatch")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:AuthnFailed")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:InvalidAttrNameOrValue")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:InvalidNameIDPolicy")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:NoAuthnContext")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:NoAvailableIDP")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:NoPassive")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:NoSupportedIDP")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:PartialLogout")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:ProxyCountExceeded")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:RequestDenied")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:RequestUnsupported")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:RequestVersionDeprecated")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:RequestVersionTooHigh")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:RequestVersionTooLow")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:ResourceNotRecognized")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:TooManyResponses")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:UnknownAttrProfile")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:UnknownPrincipal")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:status:UnsupportedBinding")]
        public void ShouldReadSamlResponseStatusCodeElement(string value)
        {
            var expected = new Uri(value);
            var xml = $@"
                    <samlp:Response xmlns:samlp=""{_protocolNamespace}"">
                      <samlp:Status>
                        <samlp:StatusCode Value=""{value}""></samlp:StatusCode>  
                      </samlp:Status>
                    </samlp:Response>";

            var response = _serializer.DeserializeSamlResponse(xml);
            Assert.NotNull(response?.Status?.StatusCode);
            Assert.Equal(expected, response.Status.StatusCode.Value);
        }

        [Fact]
        public void ShouldReadSamlResponseAssertionElement()
        {
            var expectedId = $"_{Guid.NewGuid()}";
            var expectedInstant = DateTime.UtcNow;
            var instant = expectedInstant.ToString("o");
            var expectedIssuer = "issuer";
            var expectedSubject = "subject";

            var assertionXml = $@"<saml:Assertion ID=""{expectedId}"" IssueInstant=""{instant}"" Version=""2.0"">
  <saml:Issuer>{expectedIssuer}</saml:Issuer>
  <saml:Subject>
    <saml:NameID>{expectedSubject}</saml:NameID>
  </saml:Subject>
</saml:Assertion>";
            var xml = $@"
                    <samlp:Response xmlns:samlp=""{_protocolNamespace}"" xmlns:saml=""{_assertionNamespace}"">
                      {assertionXml}
                    </samlp:Response>";
            var response = _serializer.DeserializeSamlResponse(xml);
            Assert.NotNull(response.XmlSecurityToken); // not the best test

            var token = response?.SecurityToken as Saml2SecurityToken;
            Assert.NotNull(token);

            Assert.Equal(expectedId, token.Id);
            Assert.Equal(expectedInstant, token.Assertion.IssueInstant);
            Assert.Equal(expectedIssuer, token.Issuer);
            Assert.Equal(expectedSubject, token.Assertion.Subject.NameId.Value);
        }

        [Fact]
        public void ShouldWriteAuthnRequestElementWithDefaultVersion()
        {
            var request = new AuthnRequest();

            var serialized = _serializer.SerializeAuthnRequest(request);
            Assert.NotNull(serialized);

            var doc = XDocument.Parse(serialized);
            var root = doc.Root;
            var version = root.Attribute("Version");
            Assert.Equal("samlp", doc.Root.GetPrefixOfNamespace(root.Name.Namespace));
            Assert.Equal("AuthnRequest", root.Name.LocalName);
            Assert.Equal(_protocolNamespace, root.Name.Namespace);
            Assert.NotNull(version?.Value);
            Assert.Equal("2.0", version.Value);
        }

        [Fact]
        public void ShouldWriteAuthnRequestVersionAttribute()
        {
            var expected = $"{Guid.NewGuid()}";
            var request = new AuthnRequest
            {
                Version = expected
            };

            var serialized = _serializer.SerializeAuthnRequest(request);
            Assert.NotNull(serialized);

            var doc = XDocument.Parse(serialized);
            var root = doc.Root;
            var attribute = root.Attribute("Version");
            Assert.NotNull(attribute?.Value);
            Assert.Equal(expected, attribute.Value);
        }

        [Fact]
        public void ShouldWriteAuthnRequestIdAttribute()
        {
            var expected = $"_{Guid.NewGuid()}";
            var request = new AuthnRequest
            {
                Id = expected
            };

            var serialized = _serializer.SerializeAuthnRequest(request);
            Assert.NotNull(serialized);

            var doc = XDocument.Parse(serialized);
            var root = doc.Root;
            var attribute = root.Attribute("ID");
            Assert.NotNull(attribute?.Value);
            Assert.Equal(expected, attribute.Value);
        }

        [Fact]
        public void ShouldWriteAuthnRequestIssueInstantAttribute()
        {
            var now = DateTime.UtcNow;
            var expected = now.ToString("o");
            var request = new AuthnRequest
            {
                IssueInstant = now
            };

            var serialized = _serializer.SerializeAuthnRequest(request);
            Assert.NotNull(serialized);

            var doc = XDocument.Parse(serialized);
            var root = doc.Root;
            var attribute = root.Attribute("IssueInstant");
            Assert.NotNull(attribute?.Value);
            Assert.Equal(expected, attribute.Value);
        }

        [Fact]
        public void ShouldWriteAuthnRequestProviderNameAttribute()
        {
            var expected = $"Some provider - {Guid.NewGuid()}";
            var request = new AuthnRequest
            {
                ProviderName = expected
            };

            var serialized = _serializer.SerializeAuthnRequest(request);
            Assert.NotNull(serialized);

            var doc = XDocument.Parse(serialized);
            var root = doc.Root;
            var attribute = root.Attribute("ProviderName");
            Assert.NotNull(attribute?.Value);
            Assert.Equal(expected, attribute.Value);
        }

        [Theory]
        [InlineData("https://notused")]
        public void ShouldWriteAuthnRequestDestinationAttribute(string expected)
        {
            var url = new Uri(expected);
            var request = new AuthnRequest
            {
                Destination = url
            };

            var serialized = _serializer.SerializeAuthnRequest(request);
            Assert.NotNull(serialized);

            var doc = XDocument.Parse(serialized);
            var root = doc.Root;
            var attribute = root.Attribute("Destination");
            Assert.NotNull(attribute?.Value);
            Assert.Equal(expected, attribute.Value);
        }

        [Theory]
        [InlineData("urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST")]
        public void ShouldWriteAuthnRequestProtocolBindingAttribute(string expected)
        {
            var request = new AuthnRequest
            {
                ProtocolBinding = expected
            };

            var serialized = _serializer.SerializeAuthnRequest(request);
            Assert.NotNull(serialized);

            var doc = XDocument.Parse(serialized);
            var root = doc.Root;
            var attribute = root.Attribute("ProtocolBinding");
            Assert.NotNull(attribute?.Value);
            Assert.Equal(expected, attribute.Value);
        }

        [Theory]
        [InlineData("https://notused")]
        public void ShouldWriteAuthnRequestAssertionConsumerServiceUrlAttribute(string expected)
        {
            var url = new Uri(expected);
            var request = new AuthnRequest
            {
                AssertionConsumerServiceUrl = url
            };

            var serialized = _serializer.SerializeAuthnRequest(request);
            Assert.NotNull(serialized);

            var doc = XDocument.Parse(serialized);
            var root = doc.Root;
            var attribute = root.Attribute("AssertionConsumerServiceURL");
            Assert.NotNull(attribute?.Value);
            Assert.Equal(expected, attribute.Value);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ShouldWriteAuthnRequestForceAuthnAttribute(bool expected)
        {        
            var request = new AuthnRequest
            {
                ForceAuthn = expected
            };

            var serialized = _serializer.SerializeAuthnRequest(request);
            Assert.NotNull(serialized);

            var doc = XDocument.Parse(serialized);
            var root = doc.Root;
            var attribute = root.Attribute("ForceAuthn");
            Assert.NotNull(attribute?.Value);
            Assert.True(bool.TryParse(attribute?.Value, out var forceAuthn));
            Assert.Equal(expected, forceAuthn);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ShouldWriteAuthnRequestIsPassiveAttribute(bool expected)
        {
            var request = new AuthnRequest
            {
                IsPassive = expected
            };

            var serialized = _serializer.SerializeAuthnRequest(request);
            Assert.NotNull(serialized);

            var doc = XDocument.Parse(serialized);
            var root = doc.Root;
            var attribute = root.Attribute("IsPassive");
            Assert.NotNull(attribute?.Value);
            Assert.True(bool.TryParse(attribute?.Value, out var isPassive));
            Assert.Equal(expected, isPassive);
        }

        [Fact]
        public void ShouldWriteAuthnRequestIssuerElement()
        {
            var expected = $"uri:{Guid.NewGuid()}";
            var request = new AuthnRequest
            {
                Issuer = expected
            };

            var serialized = _serializer.SerializeAuthnRequest(request);
            Assert.NotNull(serialized);

            var doc = XDocument.Parse(serialized);
            var root = doc.Root;
            var issuer = root.Element(XName.Get("Issuer", _assertionNamespace));
            Assert.NotNull(issuer);
            Assert.Equal("saml", issuer.GetPrefixOfNamespace(issuer.Name.NamespaceName));
            Assert.Equal(expected, issuer.Value);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        [InlineData(null)]
        public void ShouldWriteAuthnRequestNameIdPolicyAllowCreateAttribute(bool? expected)
        {
            var request = new AuthnRequest
            {
                NameIdPolicy = new NameIdPolicy
                {
                    AllowCreate = expected
                }
            };

            var serialized = _serializer.SerializeAuthnRequest(request);
            Assert.NotNull(serialized);

            var doc = XDocument.Parse(serialized);
            var root = doc.Root;
            var policy = root.Element(XName.Get("NameIDPolicy", _protocolNamespace));
            Assert.NotNull(policy);
            var attribute = policy.Attribute("AllowCreate");
            if (expected == null)
            {
                Assert.Null(attribute);
            }
            else
            {
                Assert.NotNull(attribute?.Value);
                Assert.True(bool.TryParse(attribute?.Value, out var allowCreate));
                Assert.Equal(expected.Value, allowCreate);
            }
        }


        [Theory]
        [InlineData("urn:oasis:names:tc:SAML:2.0:nameid-format:persistent")]
        [InlineData("urn:oasis:names:tc:SAML:1.1:nameid-format:emailAddress")]
        [InlineData("urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:nameid-format:transient")]
        public void ShouldWriteAuthnRequestNameIdPolicyFormatAttribute(string expected)
        {
            var request = new AuthnRequest
            {
                NameIdPolicy = new NameIdPolicy
                {
                    Format = expected
                }
            };

            var serialized = _serializer.SerializeAuthnRequest(request);
            Assert.NotNull(serialized);

            var doc = XDocument.Parse(serialized);
            var root = doc.Root;
            var policy = root.Element(XName.Get("NameIDPolicy", _protocolNamespace));
            Assert.NotNull(policy);
            var attribute = policy.Attribute("Format");
            Assert.NotNull(attribute?.Value);
            Assert.Equal(expected, attribute?.Value);
        }

        [Fact]
        public void ShouldWriteAuthnRequestRequestedAuthnContextDefaultComparisonAttribute()
        {
            var expected = Comparison.Exact;
            var request = new AuthnRequest
            {
                RequestedAuthnContext = new RequestedAuthnContext()
            };

            var serialized = _serializer.SerializeAuthnRequest(request);
            Assert.NotNull(serialized);

            var doc = XDocument.Parse(serialized);
            var root = doc.Root;
            var context = root.Element(XName.Get("RequestedAuthnContext", _protocolNamespace));
            Assert.NotNull(context);
            var attribute = context.Attribute("Comparison");
            Assert.NotNull(attribute?.Value);
            Assert.True(Enum.TryParse<Comparison>(attribute?.Value, true, out var comparison));
            Assert.Equal(expected, comparison);
        }

        [Theory]
        [InlineData(Comparison.Exact)]
        [InlineData(Comparison.Better)]
        [InlineData(Comparison.Minimum)]
        [InlineData(Comparison.Maximum)]
        public void ShouldWriteAuthnRequestRequestedAuthnContextComparisonAttribute(Comparison expected)
        {
            var request = new AuthnRequest
            {
                RequestedAuthnContext = new RequestedAuthnContext
                {
                    Comparison = expected
                }
            };

            var serialized = _serializer.SerializeAuthnRequest(request);
            Assert.NotNull(serialized);

            var doc = XDocument.Parse(serialized);
            var root = doc.Root;
            var context = root.Element(XName.Get("RequestedAuthnContext", _protocolNamespace));
            Assert.NotNull(context);
            var attribute = context.Attribute("Comparison");
            Assert.NotNull(attribute?.Value);
            Assert.True(Enum.TryParse<Comparison>(attribute?.Value, true, out var comparison));
            Assert.Equal(expected, comparison);
        }

        [Theory]
        [InlineData("urn:oasis:names:tc:SAML:2.0:ac:classes:Password")]
        public void ShouldWriteAuthnRequestRequestedAuthnContextClassRefElement(string expected)
        {
            var uri = new Uri(expected);
            var request = new AuthnRequest
            {
                RequestedAuthnContext = new RequestedAuthnContext
                {
                    AuthnContextClassRef = uri
                }
            };

            var serialized = _serializer.SerializeAuthnRequest(request);
            Assert.NotNull(serialized);

            var doc = XDocument.Parse(serialized);
            var root = doc.Root;
            var context = root.Element(XName.Get("RequestedAuthnContext", _protocolNamespace));
            Assert.NotNull(context);
            var element = context.Element(XName.Get("AuthnContextClassRef", _assertionNamespace));
            Assert.NotNull(element?.Value);
            Assert.Equal(expected, element.Value);
        }

        [Theory]
        [InlineData("name/password/uri")]
        public void ShouldWriteAuthnRequestRequestedAuthnContextDeclRefElement(string expected)
        {
            var request = new AuthnRequest
            {
                RequestedAuthnContext = new RequestedAuthnContext
                {
                    AuthnContextDeclRef = expected
                }
            };

            var serialized = _serializer.SerializeAuthnRequest(request);
            Assert.NotNull(serialized);

            var doc = XDocument.Parse(serialized);
            var root = doc.Root;
            var context = root.Element(XName.Get("RequestedAuthnContext", _protocolNamespace));
            Assert.NotNull(context);
            var element = context.Element(XName.Get("AuthnContextDeclRef", _assertionNamespace));
            Assert.NotNull(element?.Value);
            Assert.Equal(expected, element.Value);
        }

        [Fact]
        public void ShouldIgnoreInvalidRequest()
        {
            var invalid = $"invalid";

            var request = _serializer.DeserializeAuthnRequest(invalid);
            Assert.Null(request);
        }

        [Fact]
        public void ShouldReadAuthnRequestElement()
        {
            var xml = $"<samlp:AuthnRequest xmlns:samlp=\"{_protocolNamespace}\"></samlp:AuthnRequest>";

            var request = _serializer.DeserializeAuthnRequest(xml);
            Assert.NotNull(request);
        }

        [Fact]
        public void ShouldReadAuthnRequestSignature()
        {
            using var key = RSA.Create(2048);
            var xml = $"<samlp:AuthnRequest xmlns:samlp=\"{_protocolNamespace}\"></samlp:AuthnRequest>";
            var document = new XmlDocument();
            document.LoadXml(xml);
            document.SignXml(key);

            var signed = document.OuterXml;

            var request = _serializer.DeserializeAuthnRequest(signed);
            Assert.NotNull(request.Signature);
        }

        [Theory]
        [InlineData("2.0")]
        public void ShouldReadAuthnRequestVersionAttribute(string expected)
        {
            var xml = $"<samlp:AuthnRequest xmlns:samlp=\"{_protocolNamespace}\" Version=\"{expected}\"></samlp:AuthnRequest>";

            var request = _serializer.DeserializeAuthnRequest(xml);
            Assert.NotNull(request?.Version);
            Assert.Equal(expected, request.Version);
        }

        [Fact]
        public void ShouldReadAuthnRequestIdAttribute()
        {
            var expected = $"_{Guid.NewGuid()}";
            var xml = $"<samlp:AuthnRequest xmlns:samlp=\"{_protocolNamespace}\" ID=\"{expected}\"></samlp:AuthnRequest>";

            var request = _serializer.DeserializeAuthnRequest(xml);
            Assert.NotNull(request?.Id);
            Assert.Equal(expected, request.Id);
        }

        [Fact]
        public void ShouldReadAuthnRequestIssueInstantAttribute()
        {
            var expected = DateTime.UtcNow;
            var instant = expected.ToString("o");
            var xml = $"<samlp:AuthnRequest xmlns:samlp=\"{_protocolNamespace}\" IssueInstant=\"{instant}\"></samlp:AuthnRequest>";

            var request = _serializer.DeserializeAuthnRequest(xml);
            Assert.NotNull(request?.IssueInstant);
            Assert.Equal(expected, request.IssueInstant);
        }

        [Fact]
        public void ShouldReadAuthnRequestProviderNameAttribute()
        {
            var expected = $"Some provider - {Guid.NewGuid()}";
            var xml = $"<samlp:AuthnRequest xmlns:samlp=\"{_protocolNamespace}\" ProviderName=\"{expected}\"></samlp:AuthnRequest>";

            var request = _serializer.DeserializeAuthnRequest(xml);
            Assert.NotNull(request?.ProviderName);
            Assert.Equal(expected, request.ProviderName);
        }

        [Theory]
        [InlineData("https://notused")]
        public void ShouldReadAuthnRequestDestinationAttribute(string destination)
        {
            var expected = new Uri(destination);
            var xml = $"<samlp:AuthnRequest xmlns:samlp=\"{_protocolNamespace}\" Destination=\"{destination}\"></samlp:AuthnRequest>";

            var request = _serializer.DeserializeAuthnRequest(xml);
            Assert.NotNull(request?.Destination);
            Assert.Equal(expected, request.Destination);
        }

        [Theory]
        [InlineData("urn:oasis:names:tc:SAML:2.0:bindings:HTTP-POST")]
        public void ShouldReadAuthnRequestProtocolBindingAttribute(string expected)
        {
            var xml = $"<samlp:AuthnRequest xmlns:samlp=\"{_protocolNamespace}\" ProtocolBinding=\"{expected}\"></samlp:AuthnRequest>";

            var request = _serializer.DeserializeAuthnRequest(xml);
            Assert.NotNull(request?.ProtocolBinding);
            Assert.Equal(expected, request.ProtocolBinding);
        }

        [Theory]
        [InlineData("https://notused")]
        public void ShouldReadAuthnRequestAssertionConsumerServiceUrlAttribute(string assertionConsumerServiceUrl)
        {
            var expected = new Uri(assertionConsumerServiceUrl);
            var xml = $"<samlp:AuthnRequest xmlns:samlp=\"{_protocolNamespace}\" AssertionConsumerServiceURL=\"{assertionConsumerServiceUrl}\"></samlp:AuthnRequest>";

            var request = _serializer.DeserializeAuthnRequest(xml);
            Assert.NotNull(request?.AssertionConsumerServiceUrl);
            Assert.Equal(expected, request.AssertionConsumerServiceUrl);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ShouldReadAuthnRequestForceAuthnAttribute(bool expected)
        {
            var xml = $"<samlp:AuthnRequest xmlns:samlp=\"{_protocolNamespace}\" ForceAuthn=\"{expected}\"></samlp:AuthnRequest>";

            var request = _serializer.DeserializeAuthnRequest(xml);
            Assert.NotNull(request?.ForceAuthn);
            Assert.Equal(expected, request.ForceAuthn);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ShouldReadAuthnRequestIsPassiveAttribute(bool expected)
        {
            var xml = $"<samlp:AuthnRequest xmlns:samlp=\"{_protocolNamespace}\" IsPassive=\"{expected}\"></samlp:AuthnRequest>";

            var request = _serializer.DeserializeAuthnRequest(xml);
            Assert.NotNull(request?.IsPassive);
            Assert.Equal(expected, request.IsPassive);
        }

        [Fact]
        public void ShouldReadAuthnRequestIssuerElement()
        {
            var expected = $"uri:{Guid.NewGuid()}";
            var xml = $@"
                <samlp:AuthnRequest xmlns:samlp=""{_protocolNamespace}"" xmlns:saml=""{_assertionNamespace}"" >
                  <saml:Issuer>{expected}</saml:Issuer>
                </samlp:AuthnRequest>";

            var request = _serializer.DeserializeAuthnRequest(xml);
            Assert.NotNull(request?.Issuer);
            Assert.Equal(expected, request.Issuer);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void ShouldReadAuthnRequestNameIdPolicyAllowCreateAttribute(bool expected)
        {
            var xml = $@"
                <samlp:AuthnRequest xmlns:samlp=""{_protocolNamespace}"">
                  <samlp:NameIDPolicy AllowCreate=""{expected}""></samlp:NameIDPolicy>
                </samlp:AuthnRequest>";

            var request = _serializer.DeserializeAuthnRequest(xml);
            Assert.NotNull(request.NameIdPolicy);
            Assert.Equal(expected, request.NameIdPolicy.AllowCreate);
        }


        [Theory]
        [InlineData("urn:oasis:names:tc:SAML:2.0:nameid-format:persistent")]
        [InlineData("urn:oasis:names:tc:SAML:1.1:nameid-format:emailAddress")]
        [InlineData("urn:oasis:names:tc:SAML:1.1:nameid-format:unspecified")]
        [InlineData("urn:oasis:names:tc:SAML:2.0:nameid-format:transient")]
        public void ShouldReadAuthnRequestNameIdPolicyFormatAttribute(string expected)
        {
            var xml = $@"
                <samlp:AuthnRequest xmlns:samlp=""{_protocolNamespace}"">
                  <samlp:NameIDPolicy Format=""{expected}""></samlp:NameIDPolicy>
                </samlp:AuthnRequest>";

            var request = _serializer.DeserializeAuthnRequest(xml);
            Assert.NotNull(request.NameIdPolicy);
            Assert.Equal(expected, request.NameIdPolicy.Format);
        }

        [Fact]
        public void ShouldReadAuthnRequestRequestedAuthnContextDefaultComparisonAttribute()
        {
            var xml = $@"
                <samlp:AuthnRequest xmlns:samlp=""{_protocolNamespace}"">
                  <samlp:RequestedAuthnContext></samlp:RequestedAuthnContext>
                </samlp:AuthnRequest>";

            var request = _serializer.DeserializeAuthnRequest(xml);
            Assert.NotNull(request.RequestedAuthnContext);
            Assert.Equal(Comparison.Exact, request.RequestedAuthnContext.Comparison);
        }

        [Theory]
        [InlineData(Comparison.Exact)]
        [InlineData(Comparison.Better)]
        [InlineData(Comparison.Minimum)]
        [InlineData(Comparison.Maximum)]
        public void ShouldReadAuthnRequestRequestedAuthnContextComparisonAttribute(Comparison expected)
        {
            var lower = expected.ToString().ToLower();
            var xml = $@"
                <samlp:AuthnRequest xmlns:samlp=""{_protocolNamespace}"">
                  <samlp:RequestedAuthnContext Comparison=""{lower}""></samlp:RequestedAuthnContext>
                </samlp:AuthnRequest>";

            var request = _serializer.DeserializeAuthnRequest(xml);
            Assert.NotNull(request.RequestedAuthnContext);
            Assert.Equal(expected, request.RequestedAuthnContext.Comparison);
        }

        [Theory]
        [InlineData("urn:oasis:names:tc:SAML:2.0:ac:classes:Password")]
        public void ShouldReadAuthnRequestRequestedAuthnContextClassRefElement(string uri)
        {
            var expected = new Uri(uri);
            var xml = $@"
                <samlp:AuthnRequest xmlns:samlp=""{_protocolNamespace}"" xmlns:saml=""{_assertionNamespace}"">
                  <samlp:RequestedAuthnContext>
                    <saml:AuthnContextClassRef>{uri}</saml:AuthnContextClassRef>
                  </samlp:RequestedAuthnContext>
                </samlp:AuthnRequest>";

            var request = _serializer.DeserializeAuthnRequest(xml);
            Assert.NotNull(request.RequestedAuthnContext);
            Assert.Equal(expected, request.RequestedAuthnContext.AuthnContextClassRef);
        }

        [Theory]
        [InlineData("name/password/uri")]
        public void ShouldReadAuthnRequestRequestedAuthnContextDeclRefElement(string expected)
        {
            var xml = $@"
                <samlp:AuthnRequest xmlns:samlp=""{_protocolNamespace}"" xmlns:saml=""{_assertionNamespace}"">
                  <samlp:RequestedAuthnContext>
                    <saml:AuthnContextDeclRef>{expected}</saml:AuthnContextDeclRef>
                  </samlp:RequestedAuthnContext>
                </samlp:AuthnRequest>";

            var request = _serializer.DeserializeAuthnRequest(xml);
            Assert.NotNull(request.RequestedAuthnContext);
            Assert.Equal(expected, request.RequestedAuthnContext.AuthnContextDeclRef);
        }

        [Fact]
        public void ShouldHandleComplexAuthnRequest()
        {
            var expected = new AuthnRequest
            {
                AssertionConsumerServiceUrl = new Uri("https://assertionconsumerserviceurl"),
                Destination = new Uri("https://destination"),
                ForceAuthn = false,
                IsPassive = true,
                Id = $"_{Guid.NewGuid()}",
                IssueInstant = DateTime.UtcNow,
                Issuer = "https://someissuer",
                ProtocolBinding = "Protocol binding",
                ProviderName = "The provider name",
                NameIdPolicy = new NameIdPolicy
                {
                    AllowCreate = false,
                    Format = "some_format"
                },
                RequestedAuthnContext = new RequestedAuthnContext
                {
                    Comparison = Comparison.Minimum,
                    AuthnContextClassRef = new Uri("urn:Password"),
                    AuthnContextDeclRef = "pass"
                }
            };
            var serialized = _serializer.SerializeAuthnRequest(expected);
            var deserialized = _serializer.DeserializeAuthnRequest(serialized);

            deserialized.Should().BeEquivalentTo(expected, o => o.IncludingNestedObjects());
        }
    }
}
