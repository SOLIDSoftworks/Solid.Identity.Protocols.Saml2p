//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Text;

//namespace Solid.Identity.Protocols.Saml2p.Options
//{
//    public class Saml2pServiceProviderOptions : Saml2pServiceProvider
//    {
//        public Saml2pServiceProviderOptions()
//        {
//            MaxClockSkew = TimeSpan.FromMinutes(5);
//        }

//        public Saml2pServiceProviderOptions AddPartner(string id, Action<PartnerSaml2pIdentityProvider> configureIdp)
//        {
//            var idp = new PartnerSaml2pIdentityProvider { Id = id };
//            configureIdp(idp);
//            IdentityProviders.Add(idp);
//            return this;
//        }

//        public Saml2pServiceProviderEvents Events { get; } = new Saml2pServiceProviderEvents();
//        internal List<PartnerSaml2pIdentityProvider> IdentityProviders { get; } = new List<PartnerSaml2pIdentityProvider>();
//    }
//}
