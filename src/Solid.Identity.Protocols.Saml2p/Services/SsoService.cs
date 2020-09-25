//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Logging;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.IdentityModel.Tokens.Saml2;
//using Solid.Identity.Protocols.Saml2p.Abstractions.Configuration;
//using Solid.Identity.Protocols.Saml2p.Abstractions.Factories;
//using Solid.Identity.Protocols.Saml2p.Abstractions.Services;
//using Solid.Identity.Protocols.Saml2p.Providers;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Security;
//using System.Security.Claims;
//using System.Text;
//using System.Threading.Tasks;
//using System.Xml;

//namespace Solid.Identity.Protocols.Saml2p.Services
//{
//    public class SsoService : ISsoService
//    {
//        private ILogger<SsoService> _logger;
//        private Saml2SecurityTokenHandler _handler;
//        private ISecurityTokenDescriptorFactory _securityTokenDescriptorFactory;
//        private ConfigurationProvider _configurationProvider;
//        private PartnerProvider _partnerProvider;

//        public SsoService(
//            ILogger<SsoService> logger,
//            Saml2SecurityTokenHandler handler,
//            ISecurityTokenDescriptorFactory securityTokenDescriptorFactory,
//            ConfigurationProvider configurationProvider,
//            PartnerProvider partnerProvider
//            )
//        {
//            _logger = logger;
//            _handler = handler;
//            _securityTokenDescriptorFactory = securityTokenDescriptorFactory;
//            _configurationProvider = configurationProvider;
//            _partnerProvider = partnerProvider;

//        }
//        public async Task InitiateSsoAsync(HttpContext context, string partnerId)
//        {
//            if (context == null) throw new ArgumentNullException(nameof(context), "Http context cannot be null");
//            if (partnerId == null) throw new ArgumentNullException(nameof(partnerId), "Partner id cannot be null");
//            if (!context.User.Identity.IsAuthenticated) throw new SecurityException("Cannot initiate sso when user isn't logged in");

//            var sp = _partnerProvider.GetServiceProvider(partnerId);
//            if (sp == null) throw new ArgumentException($"Cannot find partner service provider with id: {partnerId}");

//            var token = CreateToken(context.User.Identity as ClaimsIdentity, sp);
//            // what am I doing? lol... supposed to create an html page to return
//            WriteSamlResponse(token, context);
//        }

//        private Saml2SecurityToken CreateToken(ClaimsIdentity identity, IPartnerServiceProvider sp)
//        {
//            var descriptor = _securityTokenDescriptorFactory.CreateSecurityTokenDescriptor(identity, sp);
//            var authentication = new AuthenticationInformation(new Uri("urn:oasis:names:tc:SAML:2.0:ac:classes:unspecified"), descriptor.IssuedAt ?? DateTime.UtcNow);
//            var token = _handler.CreateToken(descriptor, authentication);
//            return token as Saml2SecurityToken;
//        }

//        private void WriteSamlResponse(Saml2SecurityToken token, HttpContext context)
//        {
//            // TODO: write headers
//            //WriteSamlResponse(token, context.Response.Body);
//        }

//        //private string GenerateSamlResponse(Saml2SecurityToken token)
//        //{
//        //    using (var inner = XmlWriter.Create(output, new XmlWriterSettings { Async = true, OmitXmlDeclaration = true }))
//        //    using (var writer = XmlDictionaryWriter.CreateDictionaryWriter(inner))
//        //    {
//        //       writer.WriteStartElement(Saml2pConstants.Namespaces.ProtocolPrefix, Saml2pConstants.Elements.Response, Saml2pConstants.Namespaces.ProtocolNamespace);
//        //       writer.WriteAttributeString(Saml2Constants.Attributes.Version, Saml2Constants.Version);
//        //       writer.WriteAttributeString(Saml2Constants.Attributes.ID, $"_{Guid.NewGuid()}");
//        //       writer.WriteAttributeString(Saml2Constants.Attributes.IssueInstant, token.Assertion.IssueInstant.ToString(Saml2pConstants.DateTimeFormat));

//        //       writer.WriteStartElement(Saml2Constants.Prefix, Saml2Constants.Elements.Issuer, Saml2Constants.Namespace);
//        //       writer.WriteString(token.Issuer);
//        //       writer.WriteEndElement();

//        //       writer.WriteStartElement(Saml2pConstants.Elements.Status, Saml2pConstants.Namespaces.ProtocolNamespace);
//        //       writer.WriteStartElement(Saml2pConstants.Elements.StatusCode, Saml2pConstants.Namespaces.ProtocolNamespace);
//        //       writer.WriteAttributeString(Saml2pConstants.Attributes.Value, Saml2pConstants.Status.Success);
//        //       writer.WriteEndElement();
//        //       writer.WriteEndElement();

//        //       _handler.WriteToken(writer, token);
//        //       writer.WriteEndElement();
//        //    }
//        //}
//    }
//}
