using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml2;
using Solid.Identity.Protocols.Saml2p.Abstractions;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Microsoft.IdentityModel.Xml;
using XmlException = System.Xml.XmlException;

namespace Solid.Identity.Protocols.Saml2p.Serialization
{
    /// <summary>
    /// A serializer designed to serialize/deserialize <see cref="AuthnRequest"/> and <see cref="SamlResponse"/>.
    /// </summary>
    public class Saml2pSerializer
    {
        /// <summary>
        /// The <see cref="Saml2SecurityTokenHandler"/> used to read/write the <see cref="Saml2SecurityToken"/>.
        /// </summary>
        protected Saml2SecurityTokenHandler SecurityTokenHandler { get; }

        /// <summary>
        /// The factory used to create <see cref="XmlReader"/> instances.
        /// </summary>
        protected IXmlReaderFactory XmlReaderFactory { get; }

        /// <summary>
        /// The factory used to create <see cref="XmlWriter"/> instances.
        /// </summary>
        protected IXmlWriterFactory XmlWriterFactory { get; }

        /// <summary>
        /// The logger.
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Creates an instance of <see cref="Saml2pSerializer"/>.
        /// </summary>
        /// <param name="handler">The <see cref="Saml2SecurityTokenHandler"/> instance used to read/write <see cref="Saml2SecurityToken"/>.</param>
        /// <param name="xmlReaderFactory">The factory used to create <see cref="XmlReader"/> instances.</param>
        /// <param name="xmlWriterFactory"> The factory used to create <see cref="XmlWriter"/> instances.</param>
        /// <param name="loggerFactory">A <see cref="ILoggerFactory"/> to create an <see cref="ILogger"/> instance.</param>
        public Saml2pSerializer(
            Saml2SecurityTokenHandler handler,
            IXmlReaderFactory xmlReaderFactory,
            IXmlWriterFactory xmlWriterFactory,
            ILoggerFactory loggerFactory)
        {
            SecurityTokenHandler = handler;
            XmlReaderFactory = xmlReaderFactory;
            XmlWriterFactory = xmlWriterFactory;
            Logger = loggerFactory.CreateLogger(GetType());
        }

        // for testing purposes
        internal Saml2pSerializer(
            Saml2SecurityTokenHandler handler,
            IXmlReaderFactory xmlReaderFactory,
            IXmlWriterFactory xmlWriterFactory,
            ILogger logger)
        {
            SecurityTokenHandler = handler;
            XmlReaderFactory = xmlReaderFactory;
            XmlWriterFactory = xmlWriterFactory;
            Logger = logger;
        }

        /// <summary>
        /// Deserialized a <see cref="SamlResponse"/> from <paramref name="xml"/>.
        /// </summary>
        /// <param name="xml">The XML string.</param>
        /// <returns>A <see cref="SamlResponse"/>.</returns>
        public SamlResponse DeserializeSamlResponse(string xml)
        {
            using (var inner = new StringReader(xml))
            using (var reader = XmlReaderFactory.CreateXmlReader(inner))
            {
                return DeserializeSamlResponse(reader);
            }
        }

        /// <summary>
        /// Deserialized a <see cref="SamlResponse"/> from <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">A <see cref="TextReader"/> instance.</param>
        /// <returns>A <see cref="SamlResponse"/>.</returns>
        public SamlResponse DeserializeSamlResponse(TextReader reader) =>
            DeserializeSamlResponse(XmlReaderFactory.CreateXmlReader(reader));

        /// <summary>
        /// Deserialized a <see cref="SamlResponse"/> from <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> containing XML.</param>
        /// <returns>A <see cref="SamlResponse"/>.</returns>
        public SamlResponse DeserializeSamlResponse(Stream stream) =>
            DeserializeSamlResponse(XmlReaderFactory.CreateXmlReader(stream));

        /// <summary>
        /// Deserialized a <see cref="SamlResponse"/> from <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">An <see cref="XmlReader"/> instance.</param>
        /// <returns>A <see cref="SamlResponse"/>.</returns>
        public SamlResponse DeserializeSamlResponse(XmlReader reader) =>
            DeserializeSamlResponse(XmlDictionaryReader.CreateDictionaryReader(reader));

        /// <summary>
        /// Deserialized a <see cref="SamlResponse"/> from a <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">An <see cref="XmlDictionaryReader"/> instance.</param>
        /// <returns>A <see cref="SamlResponse"/>.</returns>
        public virtual SamlResponse DeserializeSamlResponse(XmlDictionaryReader reader)
        {
            try
            {
                if (!reader.Read()) return null;
                if (!TryReadSamlResponse(reader, out var response)) return null;
                return response;
            }
            catch (XmlException)
            {
                return null;
            }
        }

        /// <summary>
        /// Serializes a <see cref="SamlResponse"/> to an XML string.
        /// </summary>
        /// <param name="response">The <see cref="SamlResponse"/> to serialize.</param>
        /// <returns>An XML string.</returns>
        public string SerializeSamlResponse(SamlResponse response)
        {
            using (var inner = new StringWriter())
            {
                using (var writer = XmlWriterFactory.CreateXmlWriter(inner))
                {
                    SerializeSamlResponse(writer, response);
                }
                return inner.ToString();
            }
        }
        
        /// <summary>
        /// Serializes a <see cref="SamlResponse"/> to <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> instance to write the serialized <see cref="SamlResponse"/> to.</param>
        /// <param name="response">The <see cref="SamlResponse"/> to serialize.</param>
        public void SerializeSamlResponse(XmlWriter writer, SamlResponse response) 
            => SerializeSamlResponse(XmlDictionaryWriter.CreateDictionaryWriter(writer), response);

        /// <summary>
        /// Serializes a <see cref="SamlResponse"/> to <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">The <see cref="XmlDictionaryWriter"/> instance to write the serialized <see cref="SamlResponse"/> to.</param>
        /// <param name="response">The <see cref="SamlResponse"/> to serialize.</param>
        public virtual void SerializeSamlResponse(XmlDictionaryWriter writer, SamlResponse response)
        {
            if (response.SigningCredentials != null)
                writer = new EnvelopedSignatureWriter(writer, response.SigningCredentials, response.Id);
            
            using (writer.WriteElement(Saml2pConstants.Namespaces.ProtocolPrefix, Saml2pConstants.Elements.Response, Saml2pConstants.Namespaces.ProtocolNamespace))
            {
                writer.WriteXmlnsAttribute(Saml2Constants.Prefix, Saml2Constants.Namespace);
                
                if (!string.IsNullOrEmpty(response.Id)) WriteIdAttribute(writer, response.Id);
                if (!string.IsNullOrEmpty(response.Version)) WriteVersionAttribute(writer, response.Version);
                if (response.Destination != null) WriteDestinationAttribute(writer, response.Destination);
                if (response.IssueInstant != null) WriteIssueInstantAttribute(writer, response.IssueInstant.Value);
                if (!string.IsNullOrEmpty(response.InResponseTo)) WriteInResponseToAttribute(writer, response.InResponseTo);

                if (!string.IsNullOrEmpty(response.Issuer))
                    WriteIssuerElement(writer, response.Issuer);
                if (response.Status != null)
                    WriteStatusElement(writer, response.Status);
                if (response.SecurityToken != null)
                {
                    WriteSecurityToken(writer, response.SecurityToken);
                }
            }
        }

        /// <summary>
        /// Deserialized a <see cref="AuthnRequest"/> from <paramref name="xml"/>.
        /// </summary>
        /// <param name="xml">The XML string.</param>
        /// <returns>A <see cref="AuthnRequest"/>.</returns>
        public AuthnRequest DeserializeAuthnRequest(string xml)
        {
            using (var inner = new StringReader(xml))
            using (var reader = XmlReaderFactory.CreateXmlReader(inner))
            {
                return DeserializeAuthnRequest(reader);
            }
        }

        /// <summary>
        /// Deserialized a <see cref="AuthnRequest"/> from <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">A <see cref="TextReader"/> instance.</param>
        /// <returns>A <see cref="AuthnRequest"/>.</returns>
        public AuthnRequest DeserializeAuthnRequest(TextReader reader) => 
            DeserializeAuthnRequest(XmlReaderFactory.CreateXmlReader(reader));

        /// <summary>
        /// Deserialized a <see cref="AuthnRequest"/> from <paramref name="stream"/>.
        /// </summary>
        /// <param name="stream">A <see cref="Stream"/> containing XML.</param>
        /// <returns>A <see cref="AuthnRequest"/>.</returns>
        public AuthnRequest DeserializeAuthnRequest(Stream stream) => 
            DeserializeAuthnRequest(XmlReaderFactory.CreateXmlReader(stream));

        /// <summary>
        /// Deserialized a <see cref="AuthnRequest"/> from <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">An <see cref="XmlReader"/> instance.</param>
        /// <returns>A <see cref="AuthnRequest"/>.</returns>
        public AuthnRequest DeserializeAuthnRequest(XmlReader reader) => 
            DeserializeAuthnRequest(XmlDictionaryReader.CreateDictionaryReader(reader));

        /// <summary>
        /// Deserialized a <see cref="AuthnRequest"/> from a <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">An <see cref="XmlDictionaryReader"/> instance.</param>
        /// <returns>A <see cref="AuthnRequest"/>.</returns>
        public virtual AuthnRequest DeserializeAuthnRequest(XmlDictionaryReader reader)
        {
            try
            {
                if (!reader.Read()) return null;
                if (!TryReadAuthnRequest(reader, out var request)) return null;
                return request;
            }
            catch(XmlException)
            {
                return null;
            }
        }

        /// <summary>
        /// Serializes a <see cref="AuthnRequest"/> to an XML string.
        /// </summary>
        /// <param name="request">The <see cref="AuthnRequest"/> to serialize.</param>
        /// <returns>An XML string.</returns>
        public string SerializeAuthnRequest(AuthnRequest request)
        {
            using (var inner = new StringWriter())
            {
                using (var writer = XmlWriterFactory.CreateXmlWriter(inner))
                {
                    SerializeAuthnRequest(writer, request);
                }
                return inner.ToString();
            }
        }

        /// <summary>
        /// Serializes a <see cref="AuthnRequest"/> to <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">The <see cref="XmlWriter"/> instance to write the serialized <see cref="AuthnRequest"/> to.</param>
        /// <param name="request">The <see cref="AuthnRequest"/> to serialize.</param>
        public void SerializeAuthnRequest(XmlWriter writer, AuthnRequest request) 
            => SerializeAuthnRequest(XmlDictionaryWriter.CreateDictionaryWriter(writer), request);

        /// <summary>
        /// Serializes a <see cref="AuthnRequest"/> to <paramref name="writer"/>.
        /// </summary>
        /// <param name="writer">The <see cref="XmlDictionaryWriter"/> instance to write the serialized <see cref="AuthnRequest"/> to.</param>
        /// <param name="request">The <see cref="AuthnRequest"/> to serialize.</param>
        public virtual void SerializeAuthnRequest(XmlDictionaryWriter writer, AuthnRequest request)
        {
            if (request.SigningCredentials != null)
                writer = new EnvelopedSignatureWriter(writer, request.SigningCredentials, request.Id);
            
            using (writer.WriteElement(Saml2pConstants.Namespaces.ProtocolPrefix, Saml2pConstants.Elements.AuthnRequest, Saml2pConstants.Namespaces.ProtocolNamespace))
            {
                writer.WriteXmlnsAttribute(Saml2Constants.Prefix, Saml2Constants.Namespace);

                if (!string.IsNullOrEmpty(request.Id)) WriteIdAttribute(writer, request.Id);
                if (!string.IsNullOrEmpty(request.Version)) WriteVersionAttribute(writer, request.Version);
                if (!string.IsNullOrEmpty(request.ProviderName)) WriteProviderNameAttribute(writer, request.ProviderName);
                if (!string.IsNullOrEmpty(request.ProtocolBinding)) WriteProtocolBindingAttribute(writer, request.ProtocolBinding);
                if (request.Destination != null) WriteDestinationAttribute(writer, request.Destination);
                if (request.AssertionConsumerServiceUrl != null) WriteAssertionConsumerServiceUrlAttribute(writer, request.AssertionConsumerServiceUrl);
                if (request.IssueInstant != null) WriteIssueInstantAttribute(writer, request.IssueInstant.Value);
                if (request.IsPassive != null) WriteIsPassiveAttribute(writer, request.IsPassive.Value);
                if (request.ForceAuthn != null) WriteForceAuthnAttribute(writer, request.ForceAuthn.Value);
                if (!string.IsNullOrEmpty(request.Issuer)) WriteIssuerElement(writer, request.Issuer);
                if (request.NameIdPolicy != null) WriteNameIdPolicyElement(writer, request.NameIdPolicy);
                if (request.RequestedAuthnContext != null) WriteRequestedAuthnContextElement(writer, request.RequestedAuthnContext);
            }
        }

        /// <summary>
        /// Reads the attributes for a <see cref="SamlResponse"/> objects from <paramref name="reader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="XmlDictionaryReader"/> to read from.</param>
        /// <param name="response">The response read from <paramref name="reader"/>.</param>
        /// <returns>True if reading was successful, false otherwise.</returns>
        protected virtual bool TryReadSamlResponse(XmlDictionaryReader reader, out SamlResponse response)
        {
            if (!reader.IsStartElement(Saml2pConstants.Elements.Response, Saml2pConstants.Namespaces.ProtocolNamespace))
            {
                response = null;
                return false;
            }
            var r = new SamlResponse();
            using var enveloped = new EnvelopedSignatureReader(reader);
            if (enveloped.MoveToFirstAttribute())
            {
                do
                {
                    if (!TryReadSamlResponseAttribute(enveloped, r))
                        LogUnreadableAttribute(Saml2pConstants.Elements.Response, enveloped);
                } while (enveloped.MoveToNextAttribute());
            }

            ReadSamlResponseChildElements(enveloped, r);
            
            response = r;
            response.Signature = enveloped.Signature;
            
            return true;
        }

        /// <summary>
        /// Attempts to read an attribute from &lt;Response>.
        /// </summary>
        /// <param name="reader">An <see cref="XmlDictionaryReader"/> positioned at &lt;Response>.</param>
        /// <param name="response">The <see cref="SamlResponse"/> to populate with the read attribute.</param>
        /// <returns>True if reading was successful, false otherwise.</returns>
        protected virtual bool TryReadSamlResponseAttribute(XmlDictionaryReader reader, SamlResponse response)
        {
            if (TryReadVersionAttribute(reader, out var version))
                response.Version = version;
            else if (TryReadIdAttribute(reader, out var id))
                response.Id = id;
            else if (TryReadIssueInstantAttribute(reader, out var instant))
                response.IssueInstant = instant;
            else if (TryReadDestinationAttribute(reader, out var destination))
                response.Destination = destination;
            else if (TryReadInResponseToAttribute(reader, out var inResponseTo))
                response.InResponseTo = inResponseTo;
            else
                return false;
            return true;
        }

        /// <summary>
        /// Reads all child elements from &lt;Response>.
        /// </summary>
        /// <param name="reader">An <see cref="XmlDictionaryReader"/> positioned at a child element of &lt;Response>.</param>
        /// <param name="response">The <see cref="SamlResponse"/> to populate with the child elements.</param>
        protected virtual void ReadSamlResponseChildElements(XmlDictionaryReader reader, SamlResponse response)
        {
            foreach(var child in reader.GetChildElementReaders())
            {
                if (!TryReadSamlResponseChildElement(child, response))
                    LogUnreadableElement(Saml2pConstants.Elements.Response, child);
            }
        }

        /// <summary>
        /// Attempts to read a child element from &lt;Response>.
        /// </summary>
        /// <param name="reader">An <see cref="XmlDictionaryReader"/> positioned at a child element of &lt;Response>.</param>
        /// <param name="response">The <see cref="SamlResponse"/> to populate with the child element.</param>
        /// <returns>True if reading was successful, false otherwise.</returns>
        protected virtual bool TryReadSamlResponseChildElement(XmlDictionaryReader reader, SamlResponse response)
        {
            if (TryReadIssuerElement(reader, out var issuer))
                response.Issuer = issuer;
            else if (TryReadStatusElement(reader, out var status))
                response.Status = status;
            else if (TryReadSecurityToken(reader, out var token, out var xml))
            {
                response.SecurityToken = token;
                response.XmlSecurityToken = xml;
            }
            else
                return false;

            return true;
        }

        /// <summary>
        /// Attempts to read &lt;AuthnRequest> from an <see cref="XmlDictionaryReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="XmlDictionaryReader"/> positioned at an &lt;AuthnRequest> element.</param>
        /// <param name="request">The <see cref="AuthnRequest"/> output.</param>
        /// <returns>True if reading was successful, false otherwise.</returns>
        protected virtual bool TryReadAuthnRequest(XmlDictionaryReader reader, out AuthnRequest request)
        {
            if (!reader.IsStartElement(Saml2pConstants.Elements.AuthnRequest, Saml2pConstants.Namespaces.ProtocolNamespace))
            {
                request = null;
                return false;
            }

            using var enveloped = new EnvelopedSignatureReader(reader);
            var r = new AuthnRequest();
            if (enveloped.MoveToFirstAttribute())
            {
                do
                {
                    if (!TryReadAuthnRequestAttribute(enveloped, r))
                        LogUnreadableAttribute(Saml2pConstants.Elements.AuthnRequest, enveloped);
                } while (enveloped.MoveToNextAttribute());
            }

            ReadAuthnRequestChildElements(enveloped, r);

            request = r;
            request.Signature = enveloped.Signature;
            return true;
        }

        /// <summary>
        /// Attempts to read an attribute from &lt;AuthnRequest>.
        /// </summary>
        /// <param name="reader">An <see cref="XmlDictionaryReader"/> positioned at &lt;AuthnRequest>.</param>
        /// <param name="request">The <see cref="AuthnRequest"/> to populate with the read attribute.</param>
        /// <returns>True if reading was successful, false otherwise.</returns>
        protected virtual bool TryReadAuthnRequestAttribute(XmlDictionaryReader reader, AuthnRequest request)
        {
            if (TryReadVersionAttribute(reader, out var version))
                request.Version = version;
            else if (TryReadIdAttribute(reader, out var id))
                request.Id = id;
            else if (TryReadIssueInstantAttribute(reader, out var instant))
                request.IssueInstant = instant;
            else if (TryReadAssertionConsumerServiceUrlAttribute(reader, out var assertionConsumerServiceUrl))
                request.AssertionConsumerServiceUrl = assertionConsumerServiceUrl;
            else if (TryReadDestinationAttribute(reader, out var destination))
                request.Destination = destination;
            else if (TryReadForceAuthnAttribute(reader, out var forceAuthn))
                request.ForceAuthn = forceAuthn;
            else if (TryReadIsPassiveAttribute(reader, out var isPassive))
                request.IsPassive = isPassive;
            else if (TryReadProtocolBindingAttribute(reader, out var protocolBinding))
                request.ProtocolBinding = protocolBinding;
            else if (TryReadProviderNameAttribute(reader, out var providerName))
                request.ProviderName = providerName;
            else
                return false;
            return true;
        }

        /// <summary>
        /// Attempts to read an InResponseTo attribute.
        /// </summary>
        /// <param name="reader">An <see cref="XmlDictionaryReader"/> positioned at an InResponseTo attribute.</param>
        /// <param name="inResponseTo">The output attribute value.</param>
        /// <returns>True if reading was successful, false otherwise.</returns>
        protected virtual bool TryReadInResponseToAttribute(XmlDictionaryReader reader, out string inResponseTo) =>
            TryReadStringAttribute(reader, Saml2Constants.Attributes.InResponseTo, out inResponseTo);
        
        /// <summary>
        /// Attempts to read an Version attribute.
        /// </summary>
        /// <param name="reader">An <see cref="XmlDictionaryReader"/> positioned at an Version attribute.</param>
        /// <param name="version">The output attribute value.</param>
        /// <returns>True if reading was successful, false otherwise.</returns>
        protected virtual bool TryReadVersionAttribute(XmlDictionaryReader reader, out string version) =>
            TryReadStringAttribute(reader, Saml2Constants.Attributes.Version, out version);

        /// <summary>
        /// Attempts to read an ID attribute.
        /// </summary>
        /// <param name="reader">An <see cref="XmlDictionaryReader"/> positioned at an ID attribute.</param>
        /// <param name="id">The output attribute value.</param>
        /// <returns>True if reading was successful, false otherwise.</returns>
        protected virtual bool TryReadIdAttribute(XmlDictionaryReader reader, out string id) =>
            TryReadStringAttribute(reader, Saml2Constants.Attributes.ID, out id);

        /// <summary>
        /// Attempts to read an ProtocolBinding attribute.
        /// </summary>
        /// <param name="reader">An <see cref="XmlDictionaryReader"/> positioned at an ProtocolBinding attribute.</param>
        /// <param name="protocolBinding">The output attribute value.</param>
        /// <returns>True if reading was successful, false otherwise.</returns>
        protected virtual bool TryReadProtocolBindingAttribute(XmlDictionaryReader reader, out string protocolBinding) =>
            TryReadStringAttribute(reader, Saml2pConstants.Attributes.ProtocolBinding, out protocolBinding);

        /// <summary>
        /// Attempts to read an ProviderName attribute.
        /// </summary>
        /// <param name="reader">An <see cref="XmlDictionaryReader"/> positioned at an ProviderName attribute.</param>
        /// <param name="providerName">The output attribute value.</param>
        /// <returns>True if reading was successful, false otherwise.</returns>
        protected virtual bool TryReadProviderNameAttribute(XmlDictionaryReader reader, out string providerName) =>
            TryReadStringAttribute(reader, Saml2pConstants.Attributes.ProviderName, out providerName);

        /// <summary>
        /// Attempts to read an Format attribute.
        /// </summary>
        /// <param name="reader">An <see cref="XmlDictionaryReader"/> positioned at an Format attribute.</param>
        /// <param name="format">The output attribute value.</param>
        /// <returns>True if reading was successful, false otherwise.</returns>
        protected virtual bool TryReadFormatAttribute(XmlDictionaryReader reader, out string format) =>
            TryReadStringAttribute(reader, Saml2Constants.Attributes.Format, out format);

        /// <summary>
        /// Attempts to read an IssueInstant attribute.
        /// </summary>
        /// <param name="reader">An <see cref="XmlDictionaryReader"/> positioned at an IssueInstant attribute.</param>
        /// <param name="instant">The output attribute value.</param>
        /// <returns>True if reading was successful, false otherwise.</returns>
        protected virtual bool TryReadIssueInstantAttribute(XmlDictionaryReader reader, out DateTime instant) =>
            TryReadDateTimeAttribute(reader, Saml2Constants.Attributes.IssueInstant, out instant);

        /// <summary>
        /// Attempts to read an AssertionConsumerServiceURL attribute.
        /// </summary>
        /// <param name="reader">An <see cref="XmlDictionaryReader"/> positioned at an AssertionConsumerServiceURL attribute.</param>
        /// <param name="assertionConsumerServiceUrl">The output attribute value.</param>
        /// <returns>True if reading was successful, false otherwise.</returns>
        protected virtual bool TryReadAssertionConsumerServiceUrlAttribute(XmlDictionaryReader reader, out Uri assertionConsumerServiceUrl) =>
            TryReadUriAttribute(reader, Saml2pConstants.Attributes.AssertionConsumerServiceUrl, out assertionConsumerServiceUrl);

        /// <summary>
        /// Attempts to read an Destination attribute.
        /// </summary>
        /// <param name="reader">An <see cref="XmlDictionaryReader"/> positioned at an Destination attribute.</param>
        /// <param name="destination">The output attribute value.</param>
        /// <returns>True if reading was successful, false otherwise.</returns>
        protected virtual bool TryReadDestinationAttribute(XmlDictionaryReader reader, out Uri destination) =>
            TryReadUriAttribute(reader, Saml2pConstants.Attributes.Destination, out destination);

        /// <summary>
        /// Attempts to read an Value attribute.
        /// </summary>
        /// <param name="reader">An <see cref="XmlDictionaryReader"/> positioned at an Value attribute.</param>
        /// <param name="value">The output attribute value.</param>
        /// <returns>True if reading was successful, false otherwise.</returns>
        protected virtual bool TryReadValueAttribute(XmlDictionaryReader reader, out Uri value) =>
            TryReadUriAttribute(reader, Saml2pConstants.Attributes.Value, out value);

        /// <summary>
        /// Attempts to read an AllowCreate attribute.
        /// </summary>
        /// <param name="reader">An <see cref="XmlDictionaryReader"/> positioned at an AllowCreate attribute.</param>
        /// <param name="allowCreate">The output attribute value.</param>
        /// <returns>True if reading was successful, false otherwise.</returns>
        protected virtual bool TryReadAllowCreateAttribute(XmlDictionaryReader reader, out bool allowCreate) =>
            TryReadBooleanAttribute(reader, Saml2pConstants.Attributes.AllowCreate, out allowCreate);

        /// <summary>
        /// Attempts to read an ForceAuthn attribute.
        /// </summary>
        /// <param name="reader">An <see cref="XmlDictionaryReader"/> positioned at an ForceAuthn attribute.</param>
        /// <param name="forceAuthn">The output attribute value.</param>
        /// <returns>True if reading was successful, false otherwise.</returns>
        protected virtual bool TryReadForceAuthnAttribute(XmlDictionaryReader reader, out bool forceAuthn) =>
            TryReadBooleanAttribute(reader, Saml2pConstants.Attributes.ForceAuthn, out forceAuthn);

        /// <summary>
        /// Attempts to read an IsPassive attribute.
        /// </summary>
        /// <param name="reader">An <see cref="XmlDictionaryReader"/> positioned at an IsPassive attribute.</param>
        /// <param name="isPassive">The output attribute value.</param>
        /// <returns>True if reading was successful, false otherwise.</returns>
        protected virtual bool TryReadIsPassiveAttribute(XmlDictionaryReader reader, out bool isPassive) =>
            TryReadBooleanAttribute(reader, Saml2pConstants.Attributes.IsPassive, out isPassive);

        /// <summary>
        /// Attempts to read an &lt;Assertion> element.
        /// </summary>
        /// <param name="reader">An <see cref="XmlDictionaryReader"/> positioned at an &lt;Assertion> element.</param>
        /// <param name="token">The output security token.</param>
        /// <param name="xml">The output raw XML.</param>
        /// <returns>True if reading was successful, false otherwise.</returns>
        protected virtual bool TryReadSecurityToken(XmlDictionaryReader reader, out Saml2SecurityToken token, out string xml)
        {
            if (!SecurityTokenHandler.CanReadToken(reader))
            {
                token = null;
                xml = null;
            }
            else
            {
                xml = reader.ReadOuterXml();
                token = SecurityTokenHandler.ReadSaml2Token(xml);
            }
            return token != null;
        }

        /// <summary>
        /// Reads all child elements from &lt;AuthnRequest>.
        /// </summary>
        /// <param name="reader">An <see cref="XmlDictionaryReader"/> positioned at a child element of &lt;AuthnRequest>.</param>
        /// <param name="request">The <see cref="AuthnRequest"/> to populate with the child elements.</param>
        protected virtual void ReadAuthnRequestChildElements(XmlDictionaryReader reader, AuthnRequest request)
        {
            foreach(var child in reader.GetChildElementReaders())
            {
                if (!TryReadAuthnRequestChildElement(child, request))
                    LogUnreadableElement(Saml2pConstants.Elements.AuthnRequest, child);
            }
        }

        /// <summary>
        /// Attempts to read a child element from &lt;AuthnRequest>.
        /// </summary>
        /// <param name="reader">An <see cref="XmlDictionaryReader"/> positioned at a child element of &lt;AuthnRequest>.</param>
        /// <param name="request">The <see cref="AuthnRequest"/> to populate with the child element.</param>
        /// <returns>True if reading was successful, false otherwise.</returns>
        protected virtual bool TryReadAuthnRequestChildElement(XmlDictionaryReader reader, AuthnRequest request)
        {
            if (TryReadIssuerElement(reader, out var issuer))
                request.Issuer = issuer;
            else if (TryReadNameIdPolicyElement(reader, out var policy))
                request.NameIdPolicy = policy;
            else if (TryReadRequestedAuthnContextElement(reader, out var context))
                request.RequestedAuthnContext = context;
            else
                return false;

            return true;
        }

        /// <summary>
        /// Attempts to read the &lt;Issuer> element.
        /// </summary>
        /// <param name="reader">The <see cref="XmlDictionaryReader"/> used to read the XML element.</param>
        /// <param name="issuer">The <see cref="string"/> to be output as a result of reading from <paramref name="reader"/>.</param>
        /// <returns>True if reading was successful, false otherwise.</returns>
        protected virtual bool TryReadIssuerElement(XmlDictionaryReader reader, out string issuer)
        {
            var success = reader.IsStartElement(Saml2Constants.Elements.Issuer, Saml2Constants.Namespace);
            if (success)
                issuer = reader.ReadElementContentAsString();
            else
                issuer = null;
            return success;
        }

        /// <summary>
        /// Attempts to read the &lt;Status> element.
        /// </summary>
        /// <param name="reader">The <see cref="XmlDictionaryReader"/> used to read the XML element.</param>
        /// <param name="status">The <see cref="Status"/> to be output as a result of reading from <paramref name="reader"/>.</param>
        /// <returns>True if reading was successful, false otherwise.</returns>
        protected virtual bool TryReadStatusElement(XmlDictionaryReader reader, out Status status)
        {
            if(!reader.IsStartElement(Saml2pConstants.Elements.Status, Saml2pConstants.Namespaces.ProtocolNamespace))
            {
                status = null;
                return false;
            }
            status = new Status();

            foreach(var child in reader.GetChildElementReaders())
            {
                if (TryReadStatusCodeElement(child, out var statusCode))
                    status.StatusCode = statusCode;
            }

            return true;
        }

        /// <summary>
        /// Attempts to read the &lt;StatusCode> element.
        /// </summary>
        /// <param name="reader">The <see cref="XmlDictionaryReader"/> used to read the XML element.</param>
        /// <param name="statusCode">The <see cref="StatusCode"/> to be output as a result of reading from <paramref name="reader"/>.</param>
        /// <returns>True if reading was successful, false otherwise.</returns>
        protected virtual bool TryReadStatusCodeElement(XmlDictionaryReader reader, out StatusCode statusCode)
        {
            if (!reader.IsStartElement(Saml2pConstants.Elements.StatusCode, Saml2pConstants.Namespaces.ProtocolNamespace))
            {
                statusCode = null;
                return false;
            }
            statusCode = new StatusCode();

            if (reader.MoveToFirstAttribute())
            {
                do
                {
                    if (TryReadValueAttribute(reader, out var value))
                        statusCode.Value = value;
                    else
                        LogUnreadableAttribute(Saml2pConstants.Elements.StatusCode, reader);
                } while (reader.MoveToNextAttribute());
            }

            foreach(var child in reader.GetChildElementReaders())
            {
                if (TryReadStatusCodeElement(child, out var subCode))
                    statusCode.SubCode = subCode;
            }

            return true;
        }

        /// <summary>
        /// Attempts to read the &lt;NameIdPolicy> element.
        /// </summary>
        /// <param name="reader">The <see cref="XmlDictionaryReader"/> used to read the XML element.</param>
        /// <param name="policy">The <see cref="NameIdPolicy"/> to be output as a result of reading from <paramref name="reader"/>.</param>
        /// <returns>True if reading was successful, false otherwise.</returns>
        protected virtual bool TryReadNameIdPolicyElement(XmlDictionaryReader reader, out NameIdPolicy policy)
        {
            if (!reader.IsStartElement(Saml2pConstants.Elements.NameIdPolicy, Saml2pConstants.Namespaces.ProtocolNamespace))
            {
                policy = null;
                return false;
            }
            policy = new NameIdPolicy();

            if (reader.MoveToFirstAttribute())
            {
                do
                {
                    if (TryReadAllowCreateAttribute(reader, out var allowCreate))
                        policy.AllowCreate = allowCreate;
                    else if (TryReadFormatAttribute(reader, out var format))
                        policy.Format = format;
                    else
                        LogUnreadableAttribute(Saml2pConstants.Elements.NameIdPolicy, reader);
                } while (reader.MoveToNextAttribute());
            }

            return true;
        }

        /// <summary>
        /// Attempts to read the &lt;RequestedAuthnContext> element.
        /// </summary>
        /// <param name="reader">The <see cref="XmlDictionaryReader"/> used to read the XML element.</param>
        /// <param name="context">The <see cref="RequestedAuthnContext"/> to be output as a result of reading from <paramref name="reader"/>.</param>
        /// <returns>True if reading was successful, false otherwise.</returns>
        protected virtual bool TryReadRequestedAuthnContextElement(XmlDictionaryReader reader, out RequestedAuthnContext context)
        {
            if(!reader.IsStartElement(Saml2pConstants.Elements.RequestedAuthnContext, Saml2pConstants.Namespaces.ProtocolNamespace))
            {
                context = null;
                return false;
            }
            context = new RequestedAuthnContext();
            if (reader.MoveToFirstAttribute())
            {
                do
                {
                    if (reader.Name == Saml2pConstants.Attributes.Comparison && Enum.TryParse<Comparison>(reader.Value, true, out var comparison))
                        context.Comparison = comparison;
                    else
                        LogUnreadableAttribute(Saml2pConstants.Elements.RequestedAuthnContext, reader);
                } while (reader.MoveToNextAttribute());
            }
            ReadRequestedAuthnContextChildElements(reader, context);
            return true;
        }

        /// <summary>
        /// Reads all child elements from &lt;RequestedAuthnContext>.
        /// </summary>
        /// <param name="reader">An <see cref="XmlDictionaryReader"/> positioned at a child element of &lt;RequestedAuthnContext>.</param>
        /// <param name="context">The <see cref="RequestedAuthnContext"/> to populate with the child elements.</param>
        protected virtual void ReadRequestedAuthnContextChildElements(XmlDictionaryReader reader, RequestedAuthnContext context)
        {
            foreach (var child in reader.GetChildElementReaders())
            {
                if (!TryReadRequestedAuthnContextChildElement(child, context))
                    LogUnreadableElement(Saml2pConstants.Elements.RequestedAuthnContext, child);
            }
        }

        /// <summary>
        /// Attempts to read a child element from &lt;RequestedAuthnContext>.
        /// </summary>
        /// <param name="reader">An <see cref="XmlDictionaryReader"/> positioned at a child element of &lt;RequestedAuthnContext>.</param>
        /// <param name="context">The <see cref="RequestedAuthnContext"/> to populate with the child element.</param>
        /// <returns>True if reading was successful, false otherwise.</returns>
        protected virtual bool TryReadRequestedAuthnContextChildElement(XmlDictionaryReader reader, RequestedAuthnContext context)
        {
            if (TryReadAuthContextClassRefElement(reader, out var authnContextClassRef))
                context.AuthnContextClassRef = authnContextClassRef;
            else if (TryReadAuthnContextDeclRefElement(reader, out var authnContextDeclRef))
                context.AuthnContextDeclRef = authnContextDeclRef;
            else
                return false;

            return true;
        }

        /// <summary>
        /// Attempts to read the &lt;AuthnContextDeclRef> element.
        /// </summary>
        /// <param name="reader">The <see cref="XmlDictionaryReader"/> used to read the XML element.</param>
        /// <param name="authnContextDeclRef">The <see cref="string"/> to be output as a result of reading from <paramref name="reader"/>.</param>
        /// <returns>True if reading was successful, false otherwise.</returns>
        protected virtual bool TryReadAuthnContextDeclRefElement(XmlDictionaryReader reader, out string authnContextDeclRef)
        {
            authnContextDeclRef = null;
            if(!reader.IsStartElement(Saml2Constants.Elements.AuthnContextDeclRef, Saml2Constants.Namespace))
                return false;

            do
            {
                if (reader.NodeType == XmlNodeType.Text)
                    authnContextDeclRef = reader.Value;
            } while (reader.Read() && reader.NodeType != XmlNodeType.EndElement);

            return true;
        }

        /// <summary>
        /// Attempts to read the &lt;AuthnContextClassRef> element.
        /// </summary>
        /// <param name="reader">The <see cref="XmlDictionaryReader"/> used to read the XML element.</param>
        /// <param name="authnContextClassRef">The <see cref="Uri"/> to be output as a result of reading from <paramref name="reader"/>.</param>
        /// <returns>True if reading was successful, false otherwise.</returns>
        protected virtual bool TryReadAuthContextClassRefElement(XmlDictionaryReader reader, out Uri authnContextClassRef)
        {
            authnContextClassRef = null;
            if (!reader.IsStartElement(Saml2Constants.Elements.AuthnContextClassRef, Saml2Constants.Namespace))
                return false;

            do
            {
                if (reader.NodeType == XmlNodeType.Text && Uri.TryCreate(reader.Value, UriKind.Absolute, out var temp))
                    authnContextClassRef = temp;
            } while (reader.Read() && reader.NodeType != XmlNodeType.EndElement);

            return true;
        }

        /// <summary>
        /// Writes a ProviderName attribute.
        /// </summary>
        /// <param name="writer">The <see cref="XmlDictionaryWriter"/> used to write <paramref name="providerName"/>.</param>
        /// <param name="providerName">The <see cref="string"/> to write using <paramref name="writer"/>.</param>
        protected virtual void WriteProviderNameAttribute(XmlDictionaryWriter writer, string providerName) 
            => writer.WriteAttributeString(Saml2pConstants.Attributes.ProviderName, providerName);

        /// <summary>
        /// Writes a ProtocolBinding attribute.
        /// </summary>
        /// <param name="writer">The <see cref="XmlDictionaryWriter"/> used to write <paramref name="protocolBinding"/>.</param>
        /// <param name="protocolBinding">The <see cref="string"/> to write using <paramref name="writer"/>.</param>
        protected virtual void WriteProtocolBindingAttribute(XmlDictionaryWriter writer, string protocolBinding) 
            => writer.WriteAttributeString(Saml2pConstants.Attributes.ProtocolBinding, protocolBinding);

        /// <summary>
        /// Writes a Destination attribute.
        /// </summary>
        /// <param name="writer">The <see cref="XmlDictionaryWriter"/> used to write <paramref name="destination"/>.</param>
        /// <param name="destination">The <see cref="Uri"/> to write using <paramref name="writer"/>.</param>
        protected virtual void WriteDestinationAttribute(XmlDictionaryWriter writer, Uri destination) 
            => writer.WriteAttributeString(Saml2pConstants.Attributes.Destination, destination.OriginalString);

        /// <summary>
        /// Writes an AssertionConsumerServiceURL attribute.
        /// </summary>
        /// <param name="writer">The <see cref="XmlDictionaryWriter"/> used to write <paramref name="assertionConsumerServiceUrl"/>.</param>
        /// <param name="assertionConsumerServiceUrl">The <see cref="Uri"/> to write using <paramref name="writer"/>.</param>
        protected virtual void WriteAssertionConsumerServiceUrlAttribute(XmlDictionaryWriter writer, Uri assertionConsumerServiceUrl) 
            => writer.WriteAttributeString(Saml2pConstants.Attributes.AssertionConsumerServiceUrl, assertionConsumerServiceUrl.OriginalString);

        /// <summary>
        /// Writes an IssueInstant attribute.
        /// </summary>
        /// <param name="writer">The <see cref="XmlDictionaryWriter"/> used to write <paramref name="issueInstant"/>.</param>
        /// <param name="issueInstant">The <see cref="DateTime"/> to write using <paramref name="writer"/>.</param>
        protected virtual void WriteIssueInstantAttribute(XmlDictionaryWriter writer, DateTime issueInstant) 
            => writer.WriteAttributeString(Saml2Constants.Attributes.IssueInstant, issueInstant.ToString("o"));

        /// <summary>
        /// Writes an ID attribute.
        /// </summary>
        /// <param name="writer">The <see cref="XmlDictionaryWriter"/> used to write <paramref name="id"/>.</param>
        /// <param name="id">The <see cref="string"/> to write using <paramref name="writer"/>.</param>
        protected virtual void WriteIdAttribute(XmlDictionaryWriter writer, string id) 
            => writer.WriteAttributeString(Saml2Constants.Attributes.ID, id);

        /// <summary>
        /// Writes a Version attribute.
        /// </summary>
        /// <param name="writer">The <see cref="XmlDictionaryWriter"/> used to write <paramref name="version"/>.</param>
        /// <param name="version">The <see cref="string"/> to write using <paramref name="writer"/>.</param>
        protected virtual void WriteVersionAttribute(XmlDictionaryWriter writer, string version) 
            => writer.WriteAttributeString(Saml2Constants.Attributes.Version, version);

        /// <summary>
        /// Writes a Format attribute.
        /// </summary>
        /// <param name="writer">The <see cref="XmlDictionaryWriter"/> used to write <paramref name="format"/>.</param>
        /// <param name="format">The <see cref="string"/> to write using <paramref name="writer"/>.</param>
        protected virtual void WriteFormatAttribute(XmlDictionaryWriter writer, string format) 
            => writer.WriteAttributeString(Saml2Constants.Attributes.Format, format);

        /// <summary>
        /// Writes a ForceAuthn attribute.
        /// </summary>
        /// <param name="writer">The <see cref="XmlDictionaryWriter"/> used to write <paramref name="forceAuthn"/>.</param>
        /// <param name="forceAuthn">The <see cref="bool"/> to write using <paramref name="writer"/>.</param>
        protected virtual void WriteForceAuthnAttribute(XmlDictionaryWriter writer, bool forceAuthn)
            => writer.WriteAttributeString(Saml2pConstants.Attributes.ForceAuthn, GetStringValue(forceAuthn));

        /// <summary>
        /// Writes a IsPassive attribute.
        /// </summary>
        /// <param name="writer">The <see cref="XmlDictionaryWriter"/> used to write <paramref name="isPassive"/>.</param>
        /// <param name="isPassive">The <see cref="bool"/> to write using <paramref name="writer"/>.</param>
        protected virtual void WriteIsPassiveAttribute(XmlDictionaryWriter writer, bool isPassive)
            => writer.WriteAttributeString(Saml2pConstants.Attributes.IsPassive, GetStringValue(isPassive));

        /// <summary>
        /// Writes a AllowCreate attribute.
        /// </summary>
        /// <param name="writer">The <see cref="XmlDictionaryWriter"/> used to write <paramref name="allowCreate"/>.</param>
        /// <param name="allowCreate">The <see cref="bool"/> to write using <paramref name="writer"/>.</param>
        protected virtual void WriteAllowCreateAttribute(XmlDictionaryWriter writer, bool allowCreate)
            => writer.WriteAttributeString(Saml2pConstants.Attributes.AllowCreate, GetStringValue(allowCreate));

        /// <summary>
        /// Writes a InResponseTo attribute.
        /// </summary>
        /// <param name="writer">The <see cref="XmlDictionaryWriter"/> used to write <paramref name="inResponseTo"/>.</param>
        /// <param name="inResponseTo">The <see cref="string"/> to write using <paramref name="writer"/>.</param>
        protected virtual void WriteInResponseToAttribute(XmlDictionaryWriter writer, string inResponseTo) 
            => writer.WriteAttributeString(Saml2Constants.Attributes.InResponseTo, inResponseTo);

        /// <summary>
        /// Writes a Value attribute.
        /// </summary>
        /// <param name="writer">The <see cref="XmlDictionaryWriter"/> used to write <paramref name="value"/>.</param>
        /// <param name="value">The <see cref="Uri"/> to write using <paramref name="writer"/>.</param>
        protected virtual void WriteValueAttribute(XmlDictionaryWriter writer, Uri value)
            => writer.WriteAttributeString(Saml2pConstants.Attributes.Value, value.OriginalString);

        /// <summary>
        /// Writes a Comparison attribute.
        /// </summary>
        /// <param name="writer">The <see cref="XmlDictionaryWriter"/> used to write <paramref name="comparison"/>.</param>
        /// <param name="comparison">The <see cref="Comparison"/> to write using <paramref name="writer"/>.</param>
        protected virtual void WriteComparisonAttribute(XmlDictionaryWriter writer, Comparison comparison) 
            => writer.WriteAttributeString(Saml2pConstants.Attributes.Comparison, GetStringValue(comparison));


        /// <summary>
        /// Writes a &lt;Issuer> element.
        /// </summary>
        /// <param name="writer">The <see cref="XmlDictionaryWriter"/> used to write <paramref name="issuer"/>.</param>
        /// <param name="issuer">The <see cref="string"/> to write using <paramref name="writer"/>.</param>
        protected virtual void WriteIssuerElement(XmlDictionaryWriter writer, string issuer)
        {
            using (writer.WriteElement(Saml2Constants.Elements.Issuer, Saml2Constants.Namespace))
            {
                writer.WriteValue(issuer);
            }
        }

        /// <summary>
        /// Writes a &lt;NameIdPolicy> element.
        /// </summary>
        /// <param name="writer">The <see cref="XmlDictionaryWriter"/> used to write <paramref name="nameIdPolicy"/>.</param>
        /// <param name="nameIdPolicy">The <see cref="NameIdPolicy"/> to write using <paramref name="writer"/>.</param>
        protected virtual void WriteNameIdPolicyElement(XmlDictionaryWriter writer, NameIdPolicy nameIdPolicy)
        {
            using (writer.WriteElement(Saml2pConstants.Elements.NameIdPolicy, Saml2pConstants.Namespaces.ProtocolNamespace))
            {
                if (!string.IsNullOrEmpty(nameIdPolicy.Format))
                    WriteFormatAttribute(writer, nameIdPolicy.Format);
                if (nameIdPolicy.AllowCreate != null)
                    WriteAllowCreateAttribute(writer, nameIdPolicy.AllowCreate.Value);
            }
        }

        /// <summary>
        /// Writes a &lt;RequestedAuthnContext> element.
        /// </summary>
        /// <param name="writer">The <see cref="XmlDictionaryWriter"/> used to write <paramref name="requestedAuthnContext"/>.</param>
        /// <param name="requestedAuthnContext">The <see cref="RequestedAuthnContext"/> to write using <paramref name="writer"/>.</param>
        protected virtual void WriteRequestedAuthnContextElement(XmlDictionaryWriter writer, RequestedAuthnContext requestedAuthnContext)
        {
            using (writer.WriteElement(Saml2pConstants.Elements.RequestedAuthnContext, Saml2pConstants.Namespaces.ProtocolNamespace))
            {
                WriteComparisonAttribute(writer, requestedAuthnContext.Comparison);
                if (requestedAuthnContext.AuthnContextClassRef != null)
                    WriteAuthContextClassRefElement(writer, requestedAuthnContext.AuthnContextClassRef);
                if (!string.IsNullOrEmpty(requestedAuthnContext.AuthnContextDeclRef))
                    WriteAuthContextClassDeclElement(writer, requestedAuthnContext.AuthnContextDeclRef);
            }
        }

        /// <summary>
        /// Writes a &lt;AuthContextClassDecl> element.
        /// </summary>
        /// <param name="writer">The <see cref="XmlDictionaryWriter"/> used to write <paramref name="authnContextDeclRef"/>.</param>
        /// <param name="authnContextDeclRef">The <see cref="string"/> to write using <paramref name="writer"/>.</param>
        protected virtual void WriteAuthContextClassDeclElement(XmlDictionaryWriter writer, string authnContextDeclRef)
        {
            using (writer.WriteElement(Saml2Constants.Elements.AuthnContextDeclRef, Saml2Constants.Namespace))
            {
                writer.WriteValue(authnContextDeclRef);
            }
        }

        /// <summary>
        /// Writes a &lt;AuthContextClassRef> element.
        /// </summary>
        /// <param name="writer">The <see cref="XmlDictionaryWriter"/> used to write <paramref name="authnContextClassRef"/>.</param>
        /// <param name="authnContextClassRef">The <see cref="Uri"/> to write using <paramref name="writer"/>.</param>
        protected virtual void WriteAuthContextClassRefElement(XmlDictionaryWriter writer, Uri authnContextClassRef)
        {
            using (writer.WriteElement(Saml2Constants.Elements.AuthnContextClassRef, Saml2Constants.Namespace))
            {
                writer.WriteValue(authnContextClassRef.OriginalString);
            }
        }

        /// <summary>
        /// Writes a &lt;Status> element.
        /// </summary>
        /// <param name="writer">The <see cref="XmlDictionaryWriter"/> used to write <paramref name="status"/>.</param>
        /// <param name="status">The <see cref="Status"/> to write using <paramref name="writer"/>.</param>
        protected virtual void WriteStatusElement(XmlDictionaryWriter writer, Status status)
        {
            using (writer.WriteElement(Saml2pConstants.Elements.Status, Saml2pConstants.Namespaces.ProtocolNamespace))
            {
                if (status.StatusCode != null)
                    WriteStatusCodeElement(writer, status.StatusCode);
            }
        }

        /// <summary>
        /// Writes a &lt;StatusCode> element.
        /// </summary>
        /// <param name="writer">The <see cref="XmlDictionaryWriter"/> used to write <paramref name="statusCode"/>.</param>
        /// <param name="statusCode">The <see cref="StatusCode"/> to write using <paramref name="writer"/>.</param>
        protected virtual void WriteStatusCodeElement(XmlDictionaryWriter writer, StatusCode statusCode)
        {
            using (writer.WriteElement(Saml2pConstants.Elements.StatusCode, Saml2pConstants.Namespaces.ProtocolNamespace))
            {
                if (statusCode.Value != null)
                    WriteValueAttribute(writer, statusCode.Value);
                if (statusCode.SubCode != null)
                    WriteStatusCodeElement(writer, statusCode.SubCode);
            }
        }

        /// <summary>
        /// Writes a <see cref="Saml2SecurityToken"/> using an <see cref="XmlDictionaryWriter"/>.
        /// </summary>
        /// <param name="writer">The <see cref="XmlDictionaryWriter"/> used to write <paramref name="token"/>.</param>
        /// <param name="token">The <see cref="Saml2SecurityToken"/> to write using <paramref name="writer"/>.</param>
        protected virtual void WriteSecurityToken(XmlDictionaryWriter writer, Saml2SecurityToken token) =>
            SecurityTokenHandler.WriteToken(writer, token);

        /// <summary>
        /// Attempts to read an attribute as a <see cref="DateTime"/> value.
        /// </summary>
        /// <param name="reader">The <see cref="XmlDictionaryReader"/> that is positioned on an XML element.</param>
        /// <param name="attributeName">The attribute name to read.</param>
        /// <param name="dateTime">The value of the attribute.</param>
        /// <returns>True if reading was successful, false otherwise.</returns>
        protected virtual bool TryReadDateTimeAttribute(XmlDictionaryReader reader, string attributeName, out DateTime dateTime)
        {
            var temp = DateTime.MinValue;
            var success = TryReadStringAttribute(reader, attributeName, out var value) && DateTime.TryParse(reader.Value, out temp);
            dateTime = temp;
            return success;
        }

        /// <summary>
        /// Attempts to read an attribute as a <see cref="bool"/> value.
        /// </summary>
        /// <param name="reader">The <see cref="XmlDictionaryReader"/> that is positioned on an XML element.</param>
        /// <param name="attributeName">The attribute name to read.</param>
        /// <param name="boolean">The value of the attribute.</param>
        /// <returns>True if reading was successful, false otherwise.</returns>
        protected virtual bool TryReadBooleanAttribute(XmlDictionaryReader reader, string attributeName, out bool boolean)
        {
            var temp = false;
            var success = TryReadStringAttribute(reader, attributeName, out var value) && bool.TryParse(reader.Value, out temp);
            boolean = temp;
            return success;
        }

        /// <summary>
        /// Attempts to read an attribute as a <see cref="Uri"/>.
        /// </summary>
        /// <param name="reader">The <see cref="XmlDictionaryReader"/> that is positioned on an XML element.</param>
        /// <param name="attributeName">The attribute name to read.</param>
        /// <param name="uri">The value of the attribute.</param>
        /// <param name="uriKind">The type of <see cref="Uri"/> that is allowed.</param>
        /// <returns>True if reading was successful, false otherwise.</returns>
        protected virtual bool TryReadUriAttribute(XmlDictionaryReader reader, string attributeName, out Uri uri, UriKind uriKind = UriKind.Absolute)
        {
            var temp = null as Uri;
            var success = TryReadStringAttribute(reader, attributeName, out var value) && Uri.TryCreate(value, uriKind, out temp);
            uri = temp;
            return success;
        }

        /// <summary>
        /// Attempts to read an attribute as a <see cref="string"/> value.
        /// </summary>
        /// <param name="reader">The <see cref="XmlDictionaryReader"/> that is positioned on an XML element.</param>
        /// <param name="attributeName">The attribute name to read.</param>
        /// <param name="value">The value of the attribute.</param>
        /// <returns>True if reading was successful, false otherwise.</returns>
        protected virtual bool TryReadStringAttribute(XmlDictionaryReader reader, string attributeName, out string value)
        {
            if (reader.Name != attributeName)
            {
                value = null;
                return false;
            }
            value = reader.Value;
            return true;
        }

        /// <summary>
        /// Logs an attribute name and value as unreadable.
        /// </summary>
        /// <param name="elementName">The XML element that contains the attribute.</param>
        /// <param name="reader">The <see cref="XmlDictionaryReader"/> that is positioned on an XML element.</param>
        protected virtual void LogUnreadableAttribute(string elementName, XmlDictionaryReader reader)
        {
            if (reader.Name == "xmlns" || reader.Name.StartsWith("xmlns:")) return;
            if (!Logger.IsEnabled(LogLevel.Debug)) return;

            var message = $"Attribute ignored on element '{elementName}' Saml2p deserialization: {reader.Name}={reader.Value}";
            Logger.LogDebug(message);
        }

        /// <summary>
        /// Logs an element as unreadable.
        /// </summary>
        /// <param name="parentElement">The XML element that contains the element.</param>
        /// <param name="reader">The <see cref="XmlDictionaryReader"/> that is positioned on an XML element.</param>
        protected virtual void LogUnreadableElement(string parentElement, XmlDictionaryReader reader)
        {
            if (!Logger.IsEnabled(LogLevel.Debug)) return;

            var message = $"Child element of '{parentElement}' ignored during Saml2p deserialization: {reader.Name}";
            Logger.LogDebug(message);
        }

        private string GetStringValue(Comparison comparison) => comparison.ToString().ToLower();

        private string GetStringValue(bool value) => value ? "true" : "false";
    }
}
