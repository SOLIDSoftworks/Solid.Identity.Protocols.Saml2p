using System;
using System.Collections.Generic;
using System.Text;

namespace System.Security.Claims
{
    internal static class ClaimsIdentityExtensions
    {
        public static bool TryFindFirst(this ClaimsIdentity identity, string type, out Claim claim)
        {
            claim = identity.FindFirst(type);
            return claim != null;
        }
    }
}
