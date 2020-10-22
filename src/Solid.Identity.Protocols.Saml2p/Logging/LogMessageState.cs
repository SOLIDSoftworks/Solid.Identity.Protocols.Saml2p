using Solid.Identity.Protocols.Saml2p.Logging.Converters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Solid.Identity.Protocols.Saml2p.Logging
{
    internal class LogMessageState
    {
        private static readonly JsonSerializerOptions _options;

        static LogMessageState()
        {
            _options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                MaxDepth = 5
            };
            _options.Converters.Add(new BindingTypeConverter());
            _options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            _options.Converters.Add(new PathStringConverter());
            _options.Converters.Add(new ClaimConverter());
            _options.Converters.Add(new SecurityKeyConverter());
            _options.Converters.Add(new SigningCredentialsConverter());
            _options.Converters.Add(new Saml2pIdentityProviderEventsConverter());
            _options.Converters.Add(new Saml2pServiceProviderEventsConverter());
            _options.Converters.Add(new NullableJsonConverter<TimeSpan>());
        }

        protected string Serialize<T>(T obj)
            => Serialize(obj, typeof(T));
        protected string Serialize(object obj, Type type)
            => JsonSerializer.Serialize(obj, type, _options);

        public override string ToString() => Serialize(this, GetType());
    }
}
