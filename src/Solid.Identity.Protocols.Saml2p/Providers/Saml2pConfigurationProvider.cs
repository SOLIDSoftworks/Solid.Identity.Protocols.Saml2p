using Solid.Identity.Protocols.Saml2p.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Providers
{
    public class Saml2pConfigurationProvider
    {
        private IEnumerable<Saml2pIdentityProviderOptions> _identityProviderConfigurations;
        private IEnumerable<Saml2pServiceProviderOptions> _serviceProviderConfigurations;

        public Saml2pConfigurationProvider(IEnumerable<Saml2pIdentityProviderOptions> identityProviderConfigurations, IEnumerable<Saml2pServiceProviderOptions> serviceProviderConfigurations)
        {
            _identityProviderConfigurations = identityProviderConfigurations;
            _serviceProviderConfigurations = serviceProviderConfigurations;
        }

        public IEnumerable<Saml2pIdentityProviderOptions> GetIdentityProviderConfigurations(bool includeDisabled = false) => _identityProviderConfigurations.Where(idp => idp.Enabled || includeDisabled);
        public Saml2pIdentityProviderOptions GetIdentityProviderConfiguration(string id) => GetIdentityProviderConfigurations(includeDisabled: true).FirstOrDefault(idp => idp.Id == id);
        public IEnumerable<Saml2pServiceProviderOptions> GetServiceProviderConfigurations(bool includeDisabled = false) => _serviceProviderConfigurations.Where(idp => idp.Enabled || includeDisabled);
        public Saml2pServiceProviderOptions GetServiceProviderConfiguration(string id) => GetServiceProviderConfigurations(includeDisabled: true).FirstOrDefault(idp => idp.Id == id);
    }
}
