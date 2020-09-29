using Solid.Identity.Protocols.Saml2p.Options;
using Solid.Identity.Protocols.Saml2p.Models.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Providers
{
    public class Saml2pPartnerProvider
    {
        //private IdentityProviderContext _idpContext;
        private IEnumerable<Saml2pServiceProviderOptions> _localServiceProviders;
        private IEnumerable<Saml2pIdentityProviderOptions> _localIdentityProviders;

        public Saml2pPartnerProvider(
            //IdentityProviderContext idpContext,
            IEnumerable<Saml2pServiceProviderOptions> localServiceProviders, 
            IEnumerable<Saml2pIdentityProviderOptions> localIdentityProviders)
        {
            //_idpContext = idpContext;
            _localServiceProviders = localServiceProviders;
            _localIdentityProviders = localIdentityProviders;
        }

        public PartnerSaml2pIdentityProvider GetPartnerIdentityProvider(string partnerId)
        {
            var idps = _localServiceProviders.SelectMany(sp => sp.IdentityProviders).Where(idp => idp.Id == partnerId);
            if (idps.Count() > 1)
                throw new Exception("Too many results"); // better exception or handle this earlier
            return idps.FirstOrDefault();
        }

        public PartnerSaml2pServiceProvider GetPartnerServiceProvider(string partnerId)
        {
            //if (_idpContext.CurrentProvider != null)
            //    return _idpContext.CurrentProvider.ServiceProviders.FirstOrDefault(sp => sp.Id == partnerId);

            var sps = _localIdentityProviders.SelectMany(idp => idp.ServiceProviders).Where(sp => sp.Id == partnerId);
            if (sps.Count() > 1)
                throw new Exception("Too many results"); // better exception or handle this earlier
            return sps.FirstOrDefault();
        }
    }
}
