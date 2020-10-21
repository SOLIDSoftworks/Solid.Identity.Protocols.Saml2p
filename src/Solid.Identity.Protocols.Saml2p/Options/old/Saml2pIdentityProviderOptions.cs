//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace Solid.Identity.Protocols.Saml2p.Options
//{
//    public class Saml2pIdentityProviderOptions : Saml2pIdentityProvider
//    {
//        public Saml2pIdentityProviderOptions()
//        {
//            MaxClockSkew = TimeSpan.FromMinutes(5);
//        }
//        public Saml2pIdentityProviderEvents Events { get; } = new Saml2pIdentityProviderEvents();

//        public Saml2pIdentityProviderOptions AddPartner(string id, Action<PartnerSaml2pServiceProvider> configureSp)
//        {
//            var sp = new PartnerSaml2pServiceProvider { Id = id };
//            configureSp(sp);
//            ServiceProviders.Add(sp);
//            return this;
//        }

//        // TODO: change to dictioanry
//        internal List<PartnerSaml2pServiceProvider> ServiceProviders { get; } = new List<PartnerSaml2pServiceProvider>();
//    }
//}
