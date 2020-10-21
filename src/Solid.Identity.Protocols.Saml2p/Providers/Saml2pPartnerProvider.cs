using Solid.Identity.Protocols.Saml2p.Options;
using Solid.Identity.Protocols.Saml2p.Models.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Options;
using Solid.Identity.Protocols.Saml2p.Abstractions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Solid.Identity.Protocols.Saml2p.Providers
{
    public class Saml2pPartnerProvider
    {
        private Saml2pOptions _options;
        private ILogger<Saml2pPartnerProvider> _logger;
        private ISaml2pPartnerStore _store;

        public Saml2pPartnerProvider(IOptions<Saml2pOptions> options, ILogger<Saml2pPartnerProvider> logger, ISaml2pPartnerStore store = null)
        {
            _options = options.Value;
            _logger = logger;
            _store = store;
        }

        public async ValueTask<ISaml2pIdentityProvider> GetIdentityProviderAsync(string id)
        {
            _logger.LogInformation($"Searching for partner idp: '{id}'.");
            var idp = null as ISaml2pIdentityProvider;
            if (_options.IdentityProviders.TryGetValue(id, out idp))
                _logger.LogDebug("Found partner idp in-memory.");
            else if (_store != null)
            {
                idp = await _store.GetIdentityProviderAsync(id);
                if (idp != null)
                    _logger.LogDebug("Found partner idp in store.");
            }
            if (idp != null)
                _logger.LogInformation($"Found partner idp: {idp.Name}.");
            else
                _logger.LogInformation("Could not find partner idp.");
            return idp;
        }

        public async ValueTask<ISaml2pServiceProvider> GetServiceProviderAsync(string id)
        {

            if (_options.ServiceProviders.TryGetValue(id, out var sp)) return sp;
            if(_store != null)
            {
                return await _store.GetServiceProviderAsync(id);
            }
            return null;
        }
    }
}
