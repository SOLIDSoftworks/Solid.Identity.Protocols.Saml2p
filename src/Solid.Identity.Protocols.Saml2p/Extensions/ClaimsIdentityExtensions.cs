using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Solid.IdentityModel.Xml;

namespace System.Security.Claims
{
    internal static class ClaimsIdentityExtensions
    {
        public static bool TryFindFirst(this ClaimsIdentity identity, string type, out Claim claim)
        {
            claim = identity.FindFirst(type);
            return claim != null;
        }

        public static bool TryParseAuthenticationInstant(this ClaimsIdentity identity, out DateTime? instant)
        {
            var value = identity.FindFirst(ClaimTypes.AuthenticationInstant)?.Value;
            if (string.IsNullOrWhiteSpace(value))
                return Out.False(out instant);
            if (!DateTime.TryParse(value, out var parsed))
                return Out.False(out instant);

            instant = parsed;
            return instant.HasValue;
        }
    }
}
