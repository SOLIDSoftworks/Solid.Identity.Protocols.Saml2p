using Microsoft.Extensions.Caching.Distributed;
using Solid.Identity.Protocols.Saml2p.Models;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            using var activity = CreateActivity(nameof(CacheRequestAsync));
            var json = JsonSerializer.SerializeToUtf8Bytes(request);
            return _inner.SetAsync(key, json, CreateOptions());
        }

        public Task CacheStatusAsync(string key, Status status)
        {
            using var activity = CreateActivity(nameof(CacheStatusAsync));
            var json = JsonSerializer.SerializeToUtf8Bytes(status);
            return _inner.SetAsync($"{key}_status", json, CreateOptions());
        }

        public async Task<AuthnRequest> FetchRequestAsync(string key)
        {
            using var activity = CreateActivity(nameof(FetchRequestAsync));
            var json = await _inner.GetAsync(key);
            if (json == null) return null;

            return JsonSerializer.Deserialize<AuthnRequest>(json);
        }

        public async Task<Status> FetchStatusAsync(string key)
        {
            using var activity = CreateActivity(nameof(FetchStatusAsync));
            var json = await _inner.GetAsync($"{key}_status");
            if (json == null) return null;

            return JsonSerializer.Deserialize<Status>(json);
        }

        public async Task RemoveAsync(string key)
        {
            using var activity = CreateActivity(nameof(RemoveAsync));
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

        private IDisposable CreateActivity(string name)
            => Saml2pConstants.Tracing.Cache.CreateActivity($"{nameof(Saml2pCache)}.{name}", ActivityKind.Server);
    }
}
