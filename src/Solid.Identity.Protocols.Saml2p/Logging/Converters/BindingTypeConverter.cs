using Solid.Identity.Protocols.Saml2p.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Solid.Identity.Protocols.Saml2p.Logging.Converters
{
    internal class BindingTypeConverter : JsonConverter<BindingType>
    {
        public override BindingType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }

        public override void Write(Utf8JsonWriter writer, BindingType value, JsonSerializerOptions options)
        {
            if (value == BindingType.Post)
                writer.WriteStringValue(Saml2pConstants.Bindings.Post);
            if (value == BindingType.Redirect)
                writer.WriteStringValue(Saml2pConstants.Bindings.Redirect);
        }
    }
}
