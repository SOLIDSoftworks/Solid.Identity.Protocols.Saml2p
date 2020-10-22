using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Solid.Identity.Protocols.Saml2p.Logging.Converters
{
    internal class PathStringConverter : JsonConverter<PathString>
    {
        public override PathString Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, PathString value, JsonSerializerOptions options)
        {
            if (value == null)
                writer.WriteStringValue(string.Empty);
            else
                writer.WriteStringValue(value.ToString());
        }
    }
}
