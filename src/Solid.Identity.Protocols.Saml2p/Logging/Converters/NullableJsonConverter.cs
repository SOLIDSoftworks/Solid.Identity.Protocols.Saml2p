using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Solid.Identity.Protocols.Saml2p.Logging.Converters
{
    internal class NullableJsonConverter<T> : JsonConverter<T?>
        where T : struct
    {
        public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => throw new NotSupportedException();
        public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
        {
            if (!value.HasValue) writer.WriteNullValue();
            else
            {
                var converter = options.GetConverter(value.Value.GetType()) as JsonConverter<T>;
                if (converter != null) converter.Write(writer, value.Value, options);
                else writer.WriteStringValue(value.Value.ToString());
            }
        }
    }
}
