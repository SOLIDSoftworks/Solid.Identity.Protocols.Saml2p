using Solid.Identity.Protocols.Saml2p.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.Saml2p.Logging.Converters
{
    class Saml2pServiceProviderEventsConverter : JsonConverter<Saml2pServiceProviderEvents>
    {
        public override Saml2pServiceProviderEvents Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, Saml2pServiceProviderEvents value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString(options.PropertyNamingPolicy.ConvertName(nameof(Saml2pServiceProviderEvents.OnStartSso)), Stringify(value.OnStartSso));
            writer.WriteString(options.PropertyNamingPolicy.ConvertName(nameof(Saml2pServiceProviderEvents.OnGeneratingRelayState)), Stringify(value.OnGeneratingRelayState));
            writer.WriteString(options.PropertyNamingPolicy.ConvertName(nameof(Saml2pServiceProviderEvents.OnValidatingToken)), Stringify(value.OnValidatingToken));
            writer.WriteString(options.PropertyNamingPolicy.ConvertName(nameof(Saml2pServiceProviderEvents.OnValidatedToken)), Stringify(value.OnValidatedToken));
            writer.WriteString(options.PropertyNamingPolicy.ConvertName(nameof(Saml2pServiceProviderEvents.OnFinishSso)), Stringify(value.OnFinishSso));

            writer.WriteEndObject();
        }

        private string Stringify<TContext>(Func<IServiceProvider, TContext, ValueTask> func)
        {
            var delegates = func.GetInvocationList().Length;
            if (func.Method.DeclaringType == typeof(Saml2pServiceProviderEvents) || func.Method.DeclaringType.DeclaringType == typeof(Saml2pServiceProviderEvents))
                delegates--;

            return delegates == 1 ? "1 delegate" : $"{delegates} delegates";
        }
    }
}
