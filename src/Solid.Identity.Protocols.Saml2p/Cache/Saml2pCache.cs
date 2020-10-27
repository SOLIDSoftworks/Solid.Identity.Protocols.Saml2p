using Microsoft.Extensions.Caching.Distributed;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.Saml2p.Cache
{
    internal class Saml2pCache
    {
        private IDistributedCache _inner;

        public Saml2pCache(IDistributedCache inner)
        {
            _inner = inner;
        }

        public Task CacheRequestAsync(string key, AuthnRequest request)
        {
            var json = JsonSerializer.SerializeToUtf8Bytes(request);
            return _inner.SetAsync(key, json);
        }

        public async Task<AuthnRequest> FetchRequestAsync(string key)
        {
            var json = await _inner.GetAsync(key);
            if (json == null) return null;

            return JsonSerializer.Deserialize<AuthnRequest>(json);
        }
    }
}
