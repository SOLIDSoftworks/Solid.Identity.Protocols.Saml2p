using Microsoft.Extensions.Caching.Distributed;
using Solid.Identity.Protocols.Saml2p.Models;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
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
            return _inner.SetAsync(key, json, CreateOptions());
        }

        public Task CacheStatusAsync(string key, Status status)
        {
            var json = JsonSerializer.SerializeToUtf8Bytes(status);
            return _inner.SetAsync($"{key}_status", json, CreateOptions());
        }

        public async Task<AuthnRequest> FetchRequestAsync(string key)
        {
            var json = await _inner.GetAsync(key);
            if (json == null) return null;

            return JsonSerializer.Deserialize<AuthnRequest>(json);
        }

        public async Task<Status> FetchStatusAsync(string key)
        {
            var json = await _inner.GetAsync($"{key}_status");
            if (json == null) return null;

            return JsonSerializer.Deserialize<Status>(json);
        }

        public async Task RemoveAsync(string key)
        {
            await _inner.RemoveAsync(key);
            await _inner.RemoveAsync($"{key}_status");
        }

        private DistributedCacheEntryOptions CreateOptions()
        {
            return new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };
        }
        
    }
}
