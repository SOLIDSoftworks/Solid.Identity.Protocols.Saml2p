using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Solid.Identity.Protocols.Saml2p.Logging.Converters
{
    internal class SigningCredentialsConverter : JsonConverter<SigningCredentials>
    {
        public override SigningCredentials Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, SigningCredentials value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStartObject();

            writer.WriteString(options.PropertyNamingPolicy.ConvertName(nameof(SigningCredentials.Kid)), value.Kid);

            writer.WritePropertyName(options.PropertyNamingPolicy.ConvertName("Key"));
            var keyConverter = options.GetConverter(typeof(SecurityKey)) as JsonConverter<SecurityKey>;

            if (keyConverter != null)
                keyConverter.Write(writer, value.Key, options);
            else
                writer.WriteStringValue("<unable to serialize>");

            writer.WriteString(options.PropertyNamingPolicy.ConvertName(nameof(SigningCredentials.Algorithm)), value.Algorithm);
            writer.WriteString(options.PropertyNamingPolicy.ConvertName(nameof(SigningCredentials.Digest)), value.Digest);

            writer.WriteEndObject();
        }
    }
}
