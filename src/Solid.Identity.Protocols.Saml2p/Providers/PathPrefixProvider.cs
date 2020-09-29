using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Providers
{
    internal class PathPrefixProvider
    {
        private IDictionary<string, PathString> _prefixes = new Dictionary<string, PathString>();
        public void SetPrefix(string id, PathString pathPrefix) => _prefixes[id] = pathPrefix;
        public PathString GetPrefix(string id) => _prefixes.TryGetValue(id, out var pathPrefix) ? pathPrefix : PathString.Empty;
    }
}
