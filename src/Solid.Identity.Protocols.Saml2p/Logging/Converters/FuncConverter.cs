using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Solid.Identity.Protocols.Saml2p.Logging.Converters
{
    internal class FuncConverter : JsonConverter<Delegate>
    {
        public override Delegate Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, Delegate value, JsonSerializerOptions options)
        {
        }
    }
}
