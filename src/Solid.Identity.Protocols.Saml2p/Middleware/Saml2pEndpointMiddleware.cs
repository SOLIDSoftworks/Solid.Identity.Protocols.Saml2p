using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Solid.Identity.Protocols.Saml2p.Cache;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using Solid.Identity.Protocols.Saml2p.Options;
using Solid.Identity.Protocols.Saml2p.Providers;
using Solid.Identity.Protocols.Saml2p.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Solid.Identity.Protocols.Saml2p.Middleware
{
    internal abstract class Saml2pEndpointMiddleware : IDisposable
    {
        protected Saml2pSerializer Serializer { get; }
        protected Saml2pCache Cache { get; }
        protected Saml2pPartnerProvider Partners { get; }
        protected Saml2pOptions Options { get; private set; }
        protected ILogger Logger { get; }

        private IDisposable _optionsChangeToken;

        protected Saml2pEndpointMiddleware(Saml2pSerializer serializer, Saml2pCache cache, Saml2pPartnerProvider partners, IOptionsMonitor<Saml2pOptions> monitor, ILoggerFactory factory)
        {
            Serializer = serializer;
            Cache = cache;
            Partners = partners;
            Options = monitor.CurrentValue;
            _optionsChangeToken = monitor.OnChange((options, _) => Options = options);
            Logger = factory.CreateLogger(GetType());
        }

        public abstract Task InvokeAsync(HttpContext context);

        protected string SerializeAuthnRequest(AuthnRequest request, string binding)
        {
            using (var memory = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(memory, new XmlWriterSettings { OmitXmlDeclaration = true, Indent = false, CloseOutput = false }))
                {
                    Serializer.SerializeAuthnRequest(writer, request);
                }
                memory.Position = 0;
                return Encode(memory, binding);
            }
        }

        protected string SerializeSamlResponse(SamlResponse response, string binding)
        {
            using (var memory = new MemoryStream())
            {
                using (var writer = XmlWriter.Create(memory, new XmlWriterSettings { OmitXmlDeclaration = true, Indent = false, CloseOutput = false }))
                {
                    Serializer.SerializeSamlResponse(writer, response);
                }
                memory.Position = 0;
                return Encode(memory, binding);
            }
        }

        protected bool TryGetAuthnRequest(HttpContext context, out AuthnRequest request, out string binding)
        {
            const string name = "SAMLRequest";
            var reader = GetXmlReader(context, name, out binding);
            if (reader == null)
            {
                request = null;
                return false;
            }

            if (!Options.SupportedBindings.Contains(binding))
                throw new SecurityException($"SAML2P request sent using unsupported binding ({binding}).");

            using (reader)
            {
                Logger.LogDebug($"Reading '{name}' using '{binding}' binding.");
                request = Serializer.DeserializeAuthnRequest(reader);
                request.RelayState = GetRelayState(context, binding);
                return request != null;
            }
        }

        protected bool TryGetSamlResponse(HttpContext context, out SamlResponse response, out string binding)
        {
            const string name = "SAMLResponse";
            var reader = GetXmlReader(context, name, out binding);
            if (reader == null)
            {
                response = null;
                return false;
            }

            if (!Options.SupportedBindings.Contains(binding))
                throw new SecurityException($"SAML2P response sent using unsupported binding ({binding}).");

            using (reader)
            {
                Logger.LogDebug($"Reading '{name}' using '{binding}' binding.");
                response = Serializer.DeserializeSamlResponse(reader);
                response.RelayState = GetRelayState(context, binding);
                return response != null;
            }
        }

        protected async Task ChallengeAsync(HttpContext context, string returnUrl)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                context.Response.Redirect(returnUrl);
                return;
            }

            var properties = new AuthenticationProperties
            {
                RedirectUri = returnUrl
            };
            await context.ChallengeAsync(properties);
        }

        protected string GenerateReturnUrl(HttpContext httpContext, string id)
        {
            var request = httpContext.Request;
            // PathBase is only used because we set the path prefix as pathbase in Startup
            return $"{request.PathBase}/complete?id={id}";
        }

        private XmlReader GetXmlReader(HttpContext context, string name, out string binding)
        {
            var settings = new XmlReaderSettings
            {

            };
            if (HttpMethods.IsPost(context.Request.Method) && context.Request.Form != null)
            {
                var field = context.Request.Form[name];
                if (!StringValues.IsNullOrEmpty(field))
                {
                    binding = Saml2pConstants.Bindings.Post;
                    var bytes = Convert.FromBase64String(field.ToString());
                    return XmlReader.Create(new MemoryStream(bytes), settings);
                }
            }

            var query = context.Request.Query[name];
            if(HttpMethods.IsGet(context.Request.Method) && !StringValues.IsNullOrEmpty(query))
            {
                binding = Saml2pConstants.Bindings.Redirect;
                var bytes = Base64UrlDecode(query.ToString());
                using(var memory = new MemoryStream(bytes))
                using(var stream = new DeflateStream(memory, CompressionMode.Decompress))
                {
                    var deflated = new MemoryStream();
                    stream.CopyTo(deflated);
                    deflated.Position = 0;
                    return XmlReader.Create(deflated, settings);
                }
            }

            // TODO: Add SOAP and Artifact binding if possible
            binding = null;
            return null;
        }

        private string GetRelayState(HttpContext context, string binding)
        {
            const string name = "RelayState";
            var value = StringValues.Empty;
            switch (binding)
            {
                case Saml2pConstants.Bindings.Post:
                    value = context.Request.Form[name];
                    break;
                case Saml2pConstants.Bindings.Redirect:
                    value = context.Request.Query[name];
                    break;
            }
            if (StringValues.IsNullOrEmpty(value)) return null;
            return value.ToString();
        }

        private string Encode(MemoryStream memory, string binding)
        {
            if (binding == Saml2pConstants.Bindings.Post)
            {
                return Convert.ToBase64String(memory.ToArray());
            }
            else if (binding == Saml2pConstants.Bindings.Redirect)
            {
                using (var stream = new MemoryStream())
                {
                    using (var deflate = new DeflateStream(stream, CompressionMode.Compress, true))
                    {
                        memory.CopyTo(deflate);
                    }
                    return Base64UrlEncode(stream.ToArray());
                }
            }

            throw new ArgumentException("Unsupported binding type.");
        }

        private string Base64UrlEncode(byte[] bytes)
        {
            var padding = new[] { '=' };
            var base64 = Convert.ToBase64String(bytes);
            return base64
                .TrimEnd(padding)
                .Replace('+', '-')
                .Replace('/', '_')
            ;
        }

        private byte[] Base64UrlDecode(string base64UrlEncoded)
        {
            var base64 = base64UrlEncoded
                .Replace('_', '/')
                .Replace('-', '+')
            ;
            switch (base64UrlEncoded.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
            var bytes = Convert.FromBase64String(base64);
            return bytes;
        }

        public void Dispose() => _optionsChangeToken?.Dispose();
    }
}
