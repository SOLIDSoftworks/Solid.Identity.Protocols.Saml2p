using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.Saml2p.Cache
{
    public class Saml2pCache
    {
        private IDistributedCache _inner;

        public Saml2pCache(IDistributedCache inner)
        {
            _inner = inner;
        }

        public Task CacheRequestAsync(string key, AuthnRequest request)
        {
            var json = JsonConvert.SerializeObject(request);
            return _inner.SetStringAsync(key, json);
        }

        public async Task<AuthnRequest> FetchRequestAsync(string key)
        {
            var json = await _inner.GetStringAsync(key);
            if (json == null) return null;

            return JsonConvert.DeserializeObject<AuthnRequest>(json);
        }
    }
}
