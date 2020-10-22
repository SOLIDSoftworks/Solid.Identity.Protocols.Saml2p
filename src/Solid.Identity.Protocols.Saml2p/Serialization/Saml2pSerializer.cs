using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml2;
using Solid.Identity.Protocols.Saml2p.Abstractions;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Xml;

namespace Solid.Identity.Protocols.Saml2p.Serialization
{
    public class Saml2pSerializer
    {
        protected Saml2SecurityTokenHandler Saml2SecurityTokenHandler { get; }
        protected IXmlReaderFactory XmlReaderFactory { get; }
        protected IXmlWriterFactory XmlWriterFactory { get; }
        protected ILogger<Saml2pSerializer> Logger { get; }

        public Saml2pSerializer(
            Saml2SecurityTokenHandler handler,
            IXmlReaderFactory xmlReaderFactory,
            IXmlWriterFactory xmlWriterFactory,
            ILogger<Saml2pSerializer> logger)
        {
            Saml2SecurityTokenHandler = handler;
            XmlReaderFactory = xmlReaderFactory;
            XmlWriterFactory = xmlWriterFactory;
            Logger = logger;
        }

        public SamlResponse DeserializeSamlResponse(string xml)
        {
            using (var inner = new StringReader(xml))
            using (var reader = XmlReaderFactory.CreateXmlReader(inner))
            {
                return DeserializeSamlResponse(reader);
            }
        }
        public SamlResponse DeserializeSamlResponse(TextReader reader) =>
            DeserializeSamlResponse(XmlReaderFactory.CreateXmlReader(reader));
        public SamlResponse DeserializeSamlResponse(Stream stream) =>
            DeserializeSamlResponse(XmlReaderFactory.CreateXmlReader(stream));
        public SamlResponse DeserializeSamlResponse(XmlReader reader) =>
            DeserializeSamlResponse(XmlDictionaryReader.CreateDictionaryReader(reader));
        public virtual SamlResponse DeserializeSamlResponse(XmlDictionaryReader reader)
        {
            try
            {
                if (!reader.Read()) return null;
                if (!reader.IsStartElement(Saml2pConstants.Elements.Response, Saml2pConstants.Namespaces.ProtocolNamespace)) return null;
            }
            catch (XmlException)
            {
                return null;
            }

            var response = new SamlResponse();

            ReadSamlResponse(reader, response);

            return response;
        }

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
        public void SerializeSamlResponse(XmlWriter writer, SamlResponse response) => SerializeSamlResponse(XmlDictionaryWriter.CreateDictionaryWriter(writer), response);
        public virtual void SerializeSamlResponse(XmlDictionaryWriter writer, SamlResponse response)
        {
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

        public AuthnRequest DeserializeAuthnRequest(string xml)
        {
            using (var inner = new StringReader(xml))
            using (var reader = XmlReaderFactory.CreateXmlReader(inner))
            {
                return DeserializeAuthnRequest(reader);
            }
        }
        public AuthnRequest DeserializeAuthnRequest(TextReader reader) => 
            DeserializeAuthnRequest(XmlReaderFactory.CreateXmlReader(reader));
        public AuthnRequest DeserializeAuthnRequest(Stream stream) => 
            DeserializeAuthnRequest(XmlReaderFactory.CreateXmlReader(stream));
        public AuthnRequest DeserializeAuthnRequest(XmlReader reader) => 
            DeserializeAuthnRequest(XmlDictionaryReader.CreateDictionaryReader(reader));
        public virtual AuthnRequest DeserializeAuthnRequest(XmlDictionaryReader reader)
        {
            try
            {
                if (!reader.Read()) return null;
                if (!reader.IsStartElement(Saml2pConstants.Elements.AuthnRequest, Saml2pConstants.Namespaces.ProtocolNamespace)) return null;
            }
            catch(XmlException)
            {
                return null;
            }

            var request = new AuthnRequest();

            ReadAuthnRequest(reader, request);

            return request;
        }

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
        public void SerializeAuthnRequest(XmlWriter writer, AuthnRequest request) => SerializeAuthnRequest(XmlDictionaryWriter.CreateDictionaryWriter(writer), request);
        public virtual void SerializeAuthnRequest(XmlDictionaryWriter writer, AuthnRequest request)
        {
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

                if (!string.IsNullOrEmpty(request.Issuer))
                    WriteIssuerElement(writer, request.Issuer);
                if (request.NameIdPolicy != null)
                    WriteNameIdPolicy(writer, request.NameIdPolicy);
                if (request.RequestedAuthnContext != null)
                    WriteRequestedAuthnContextElement(writer, request.RequestedAuthnContext);
            }
        }

        protected virtual void ReadSamlResponse(XmlDictionaryReader reader, SamlResponse response)
        {
            if (reader.MoveToFirstAttribute())
            {
                do
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
                        IgnoreAndLogUnparsableAttribute(Saml2pConstants.Elements.AuthnRequest, reader);
                } while (reader.MoveToNextAttribute());
            }

            ReadSamlResponseChildElements(reader, response);
        }

        protected virtual void ReadSamlResponseChildElements(XmlDictionaryReader reader, SamlResponse response)
        {
            foreach(var child in reader.GetChildElementReaders())
            {
                if (TryReadIssuerElement(child, out var issuer))
                    response.Issuer = issuer;
                else if (TryReadStatusElement(child, out var status))
                    response.Status = status;
                else if (TryReadSecurityToken(child, out var token, out var xml))
                {
                    response.SecurityToken = token;
                    response.XmlSecurityToken = xml;
                }
            }
        }

        protected virtual void ReadAuthnRequest(XmlDictionaryReader reader, AuthnRequest request)
        {
            if (reader.MoveToFirstAttribute())
            {
                do
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
                        IgnoreAndLogUnparsableAttribute(Saml2pConstants.Elements.AuthnRequest, reader);
                } while (reader.MoveToNextAttribute());
            }

            ReadAuthnRequestChildElements(reader, request);
        }

        protected virtual bool TryReadInResponseToAttribute(XmlDictionaryReader reader, out string inResponseTo) =>
            TryReadStringAttribute(reader, Saml2Constants.Attributes.InResponseTo, out inResponseTo);

        protected virtual bool TryReadVersionAttribute(XmlDictionaryReader reader, out string version) =>
            TryReadStringAttribute(reader, Saml2Constants.Attributes.Version, out version);

        protected virtual bool TryReadIdAttribute(XmlDictionaryReader reader, out string id) =>
            TryReadStringAttribute(reader, Saml2Constants.Attributes.ID, out id);

        protected virtual bool TryReadProtocolBindingAttribute(XmlDictionaryReader reader, out string protocolBinding) =>
            TryReadStringAttribute(reader, Saml2pConstants.Attributes.ProtocolBinding, out protocolBinding);

        protected virtual bool TryReadProviderNameAttribute(XmlDictionaryReader reader, out string providerName) =>
            TryReadStringAttribute(reader, Saml2pConstants.Attributes.ProviderName, out providerName);

        protected virtual bool TryReadFormatAttribute(XmlDictionaryReader reader, out string format) =>
            TryReadStringAttribute(reader, Saml2Constants.Attributes.Format, out format);

        protected virtual bool TryReadIssueInstantAttribute(XmlDictionaryReader reader, out DateTime instant) =>
            TryReadDateTimeAttribute(reader, Saml2Constants.Attributes.IssueInstant, out instant);

        protected virtual bool TryReadAssertionConsumerServiceUrlAttribute(XmlDictionaryReader reader, out Uri assertionConsumerServiceUrl) =>
            TryReadUriAttribute(reader, Saml2pConstants.Attributes.AssertionConsumerServiceUrl, out assertionConsumerServiceUrl);

        protected virtual bool TryReadDestinationAttribute(XmlDictionaryReader reader, out Uri destination) =>
            TryReadUriAttribute(reader, Saml2pConstants.Attributes.Destination, out destination);

        protected virtual bool TryReadValueAttribute(XmlDictionaryReader reader, out Uri value) =>
            TryReadUriAttribute(reader, Saml2pConstants.Attributes.Value, out value);

        protected virtual bool TryReadAllowCreateAttribute(XmlDictionaryReader reader, out bool allowCreate) =>
            TryReadBooleanAttribute(reader, Saml2pConstants.Attributes.AllowCreate, out allowCreate);

        protected virtual bool TryReadForceAuthnAttribute(XmlDictionaryReader reader, out bool forceAuthn) =>
            TryReadBooleanAttribute(reader, Saml2pConstants.Attributes.ForceAuthn, out forceAuthn);

        protected virtual bool TryReadIsPassiveAttribute(XmlDictionaryReader reader, out bool isPassive) =>
            TryReadBooleanAttribute(reader, Saml2pConstants.Attributes.IsPassive, out isPassive);

        protected virtual bool TryReadSecurityToken(XmlDictionaryReader reader, out Saml2SecurityToken token, out string xml)
        {
            if (!Saml2SecurityTokenHandler.CanReadToken(reader))
            {
                token = null;
                xml = null;
            }
            else
            {
                xml = reader.ReadOuterXml();
                token = Saml2SecurityTokenHandler.ReadSaml2Token(xml);
            }
            return token != null;
        }

        protected virtual void ReadAuthnRequestChildElements(XmlDictionaryReader reader, AuthnRequest request)
        {
            foreach(var child in reader.GetChildElementReaders())
            {
                if (TryReadIssuerElement(child, out var issuer))
                    request.Issuer = issuer;
                else if (TryReadNameIdPolicyElement(child, out var policy))
                    request.NameIdPolicy = policy;
                else if (TryReadRequestedAuthnContextElement(reader, out var context))
                    request.RequestedAuthnContext = context;
            }
        }

        protected virtual bool TryReadIssuerElement(XmlDictionaryReader reader, out string issuer)
        {
            var success = reader.IsStartElement(Saml2Constants.Elements.Issuer, Saml2Constants.Namespace);
            if (success)
                issuer = reader.ReadElementContentAsString();
            else
                issuer = null;
            return success;
        }

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
                        IgnoreAndLogUnparsableAttribute(Saml2pConstants.Elements.StatusCode, reader);
                } while (reader.MoveToNextAttribute());
            }

            foreach(var child in reader.GetChildElementReaders())
            {
                if (TryReadStatusCodeElement(child, out var subCode))
                    statusCode.SubCode = subCode;
            }

            return true;
        }

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
                        IgnoreAndLogUnparsableAttribute(Saml2pConstants.Elements.NameIdPolicy, reader);
                } while (reader.MoveToNextAttribute());
            }

            return true;
        }

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
                        IgnoreAndLogUnparsableAttribute(Saml2pConstants.Elements.RequestedAuthnContext, reader);
                } while (reader.MoveToNextAttribute());
            }
            ReadRequestAuthnContextChildElements(reader, context);
            return true;
        }

        protected virtual void ReadRequestAuthnContextChildElements(XmlDictionaryReader reader, RequestedAuthnContext context)
        {
            foreach(var child in reader.GetChildElementReaders())
            {
                if (TryReadAuthContextClassRefElement(reader, out var authnContextClassRef))
                    context.AuthnContextClassRef = authnContextClassRef;
                else if (TryReadAuthnContextDeclRefElement(reader, out var authnContextDeclRef))
                    context.AuthnContextDeclRef = authnContextDeclRef;
            }
        }

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

        protected virtual void WriteProviderNameAttribute(XmlDictionaryWriter writer, string providerName) =>
            writer.WriteAttributeString(Saml2pConstants.Attributes.ProviderName, providerName);

        protected virtual void WriteProtocolBindingAttribute(XmlDictionaryWriter writer, string protocolBinding) => 
            writer.WriteAttributeString(Saml2pConstants.Attributes.ProtocolBinding, protocolBinding);

        protected virtual void WriteDestinationAttribute(XmlDictionaryWriter writer, Uri destination) => 
            writer.WriteAttributeString(Saml2pConstants.Attributes.Destination, destination.OriginalString);

        protected virtual void WriteAssertionConsumerServiceUrlAttribute(XmlDictionaryWriter writer, Uri assertionConsumerServiceUrl) => 
            writer.WriteAttributeString(Saml2pConstants.Attributes.AssertionConsumerServiceUrl, assertionConsumerServiceUrl.OriginalString);

        protected virtual void WriteIssueInstantAttribute(XmlDictionaryWriter writer, DateTime issueInstant) => 
            writer.WriteAttributeString(Saml2Constants.Attributes.IssueInstant, issueInstant.ToString("o"));

        protected virtual void WriteIdAttribute(XmlDictionaryWriter writer, string id) =>
            writer.WriteAttributeString(Saml2Constants.Attributes.ID, id);

        protected virtual void WriteVersionAttribute(XmlDictionaryWriter writer, string version) => 
            writer.WriteAttributeString(Saml2Constants.Attributes.Version, version);

        protected virtual void WriteFormatAttribute(XmlDictionaryWriter writer, string format) =>
            writer.WriteAttributeString(Saml2Constants.Attributes.Format, format);

        protected virtual void WriteForceAuthnAttribute(XmlDictionaryWriter writer, bool forceAuthn) =>
            writer.WriteAttributeString(Saml2pConstants.Attributes.ForceAuthn, forceAuthn ? "true" : "false");

        protected virtual void WriteIsPassiveAttribute(XmlDictionaryWriter writer, bool isPassive) =>
            writer.WriteAttributeString(Saml2pConstants.Attributes.IsPassive, isPassive ? "true" : "false");

        protected virtual void WriteAllowCreateAttribute(XmlDictionaryWriter writer, bool allowCreate) =>
            writer.WriteAttributeString(Saml2pConstants.Attributes.AllowCreate, allowCreate ? "true" : "false");

        protected virtual void WriteInResponseToAttribute(XmlDictionaryWriter writer, string inResponseTo) =>
            writer.WriteAttributeString(Saml2Constants.Attributes.InResponseTo, inResponseTo);

        protected virtual void WriteValueAttribute(XmlDictionaryWriter writer, Uri value) =>
            writer.WriteAttributeString(Saml2pConstants.Attributes.Value, value.OriginalString);

        protected virtual void WriteComparisonAttribute(XmlDictionaryWriter writer, Comparison comparison) =>
            writer.WriteAttributeString(Saml2pConstants.Attributes.Comparison, GetStringValue(comparison));

        protected virtual void WriteIssuerElement(XmlDictionaryWriter writer, string issuer)
        {
            using (writer.WriteElement(Saml2Constants.Elements.Issuer, Saml2Constants.Namespace))
            {
                writer.WriteValue(issuer);
            }
        }

        protected virtual void WriteNameIdPolicy(XmlDictionaryWriter writer, NameIdPolicy nameIdPolicy)
        {
            using (writer.WriteElement(Saml2pConstants.Elements.NameIdPolicy, Saml2pConstants.Namespaces.ProtocolNamespace))
            {
                if (!string.IsNullOrEmpty(nameIdPolicy.Format))
                    WriteFormatAttribute(writer, nameIdPolicy.Format);
                if (nameIdPolicy.AllowCreate != null)
                    WriteAllowCreateAttribute(writer, nameIdPolicy.AllowCreate.Value);
            }
        }

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

        protected virtual void WriteAuthContextClassDeclElement(XmlDictionaryWriter writer, string authnContextDeclRef)
        {
            using (writer.WriteElement(Saml2Constants.Elements.AuthnContextDeclRef, Saml2Constants.Namespace))
            {
                writer.WriteValue(authnContextDeclRef);
            }
        }

        protected virtual void WriteAuthContextClassRefElement(XmlDictionaryWriter writer, Uri authnContextClassRef)
        {
            using (writer.WriteElement(Saml2Constants.Elements.AuthnContextClassRef, Saml2Constants.Namespace))
            {
                writer.WriteValue(authnContextClassRef.OriginalString);
            }
        }

        protected virtual void WriteStatusElement(XmlDictionaryWriter writer, Status status)
        {
            using (writer.WriteElement(Saml2pConstants.Elements.Status, Saml2pConstants.Namespaces.ProtocolNamespace))
            {
                if (status.StatusCode != null)
                    WriteStatusCodeElement(writer, status.StatusCode);
            }
        }

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

        protected virtual void WriteSecurityToken(XmlDictionaryWriter writer, SecurityToken token) =>
            Saml2SecurityTokenHandler.WriteToken(writer, token);

        protected virtual bool TryReadDateTimeAttribute(XmlDictionaryReader reader, string attributeName, out DateTime dateTime)
        {
            var temp = DateTime.MinValue;
            var success = TryReadStringAttribute(reader, attributeName, out var value) && DateTime.TryParse(reader.Value, out temp);
            dateTime = temp;
            return success;
        }

        protected virtual bool TryReadBooleanAttribute(XmlDictionaryReader reader, string attributeName, out bool boolean)
        {
            var temp = false;
            var success = TryReadStringAttribute(reader, attributeName, out var value) && bool.TryParse(reader.Value, out temp);
            boolean = temp;
            return success;
        }

        protected virtual bool TryReadUriAttribute(XmlDictionaryReader reader, string attributeName, out Uri uri, UriKind uriKind = UriKind.Absolute)
        {
            var temp = null as Uri;
            var success = TryReadStringAttribute(reader, attributeName, out var value) && Uri.TryCreate(value, uriKind, out temp);
            uri = temp;
            return success;
        }

        protected virtual bool TryReadStringAttribute(XmlDictionaryReader reader, string atrributeName, out string value)
        {
            if (reader.Name != atrributeName)
            {
                value = null;
                return false;
            }
            value = reader.Value;
            return true;
        }

        protected virtual void IgnoreAndLogUnparsableAttribute(string elementName, XmlDictionaryReader reader)
        {
            var message = $"Unparsable attribute: {reader.Name}={reader.Value}";
            Logger.LogDebug(message);
        }

        //protected virtual void IgnoreAndLogUnparsableElement(string elementName, XmlDictionaryReader reader)
        //{
        //    var message = $"Unparsable element: {reader.Name}";
        //    Logger.LogDebug(message);
        //}

        private string GetStringValue(Comparison comparison) => comparison.ToString().ToLower();
    }
}
