using Solid.Identity.Protocols.Saml2p.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.Saml2p.Logging.Converters
{
    internal class Saml2pIdentityProviderEventsConverter : JsonConverter<Saml2pIdentityProviderEvents>
    {
        public override Saml2pIdentityProviderEvents Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, Saml2pIdentityProviderEvents value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteString(options.PropertyNamingPolicy.ConvertName(nameof(Saml2pIdentityProviderEvents.OnAcceptSso)), Stringify(value.OnAcceptSso));
            writer.WriteString(options.PropertyNamingPolicy.ConvertName(nameof(Saml2pIdentityProviderEvents.OnInitiateSso)), Stringify(value.OnInitiateSso));
            writer.WriteString(options.PropertyNamingPolicy.ConvertName(nameof(Saml2pIdentityProviderEvents.OnCreatingSecurityToken)), Stringify(value.OnCreatingSecurityToken));
            writer.WriteString(options.PropertyNamingPolicy.ConvertName(nameof(Saml2pIdentityProviderEvents.OnCreatedSecurityToken)), Stringify(value.OnCreatedSecurityToken));
            writer.WriteString(options.PropertyNamingPolicy.ConvertName(nameof(Saml2pIdentityProviderEvents.OnCompleteSso)), Stringify(value.OnCompleteSso));

            writer.WriteEndObject();
        }

        private string Stringify<TContext>(Func<IServiceProvider, TContext, ValueTask> func)
        {
            var delegates = func.GetInvocationList().Length;
            if (func.Method.DeclaringType == typeof(Saml2pIdentityProviderEvents) || func.Method.DeclaringType.DeclaringType == typeof(Saml2pIdentityProviderEvents))
                delegates--;

            return delegates == 1 ? "1 delegate" : $"{delegates} delegates";
        }
    }
}
