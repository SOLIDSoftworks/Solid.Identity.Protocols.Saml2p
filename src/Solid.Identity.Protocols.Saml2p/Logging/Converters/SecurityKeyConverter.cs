using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Solid.Identity.Protocols.Saml2p.Logging.Converters
{
    internal class SecurityKeyConverter : JsonConverter<SecurityKey>
    {
        public override SecurityKey Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, SecurityKey value, JsonSerializerOptions options)
        {
            if(value == null)
            {
                writer.WriteNullValue();
                return;
            }

            writer.WriteStartObject();

            if(value is X509SecurityKey x509)
            {
                writer.WriteString(options.PropertyNamingPolicy.ConvertName("Name"), x509.Certificate.Subject);
                writer.WriteString(options.PropertyNamingPolicy.ConvertName("Thumbprint"), x509.Certificate.Thumbprint);
                writer.WriteString(options.PropertyNamingPolicy.ConvertName("SerialNumber"), x509.Certificate.SerialNumber);
                writer.WriteString(options.PropertyNamingPolicy.ConvertName("Expires"), x509.Certificate.NotAfter);
            }
            else
            {
                writer.WriteString(options.PropertyNamingPolicy.ConvertName("Name"), value.KeyId);
            }
            var type = value.GetType().Name;
            writer.WriteString(options.PropertyNamingPolicy.ConvertName("Type"), type);

            writer.WriteEndObject();
        }
    }
}
