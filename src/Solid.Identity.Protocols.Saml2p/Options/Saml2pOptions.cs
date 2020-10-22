using Microsoft.AspNetCore.Http;
using Solid.Identity.Protocols.Saml2p.Abstractions;
using Solid.Identity.Protocols.Saml2p.Models;
using Solid.Identity.Protocols.Saml2p.Models.Context;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.Saml2p.Options
{
    public class Saml2pOptions
    {

        public string DefaultIssuer { get; set; }
        public TimeSpan? DefaultMaxClockSkew { get; set; }
        public TimeSpan DefaultTokenLifetime { get; set; } = TimeSpan.FromMinutes(5);
        public string PartnerIdQueryParameter { get; set; } = "partnerId";
        public ICollection<BindingType> SupportedBindings { get; internal set; } = Saml2pConstants.Bindings.All;

        public PathString AcceptPath { get; set; } = PathString.Empty;

        public PathString InitiatePath { get; set; } = "/initiate";

        public PathString CompletePath { get; set; } = "/complete";
        public PathString StartPath { get; set; } = "/start";

        public PathString FinishPath { get; set; } = "/finish";

        public Saml2pIdentityProviderEvents IdentityProviderEvents { get; } = new Saml2pIdentityProviderEvents();
        public Saml2pServiceProviderEvents ServiceProviderEvents { get; } = new Saml2pServiceProviderEvents();

        public Saml2pOptions AddServiceProvider(string id, Action<Saml2pServiceProvider> configure)
        {
            var sp = new Saml2pServiceProvider { Id = id, Name = id };
            configure(sp);
            ServiceProviders[id] = sp;
            return this;
        }
        public Saml2pOptions AddIdentityProvider(string id, Action<Saml2pIdentityProvider> configure)
        {
            var sp = new Saml2pIdentityProvider { Id = id, Name = id };
            configure(sp);
            IdentityProviders[id] = sp;
            return this;
        }
        internal IDictionary<string, ISaml2pServiceProvider> ServiceProviders { get; } = new Dictionary<string, ISaml2pServiceProvider>(StringComparer.OrdinalIgnoreCase);
        internal IDictionary<string, ISaml2pIdentityProvider> IdentityProviders { get; } = new Dictionary<string, ISaml2pIdentityProvider>(StringComparer.OrdinalIgnoreCase);

        //internal IDictionary<string, IIdentityProvider> IdentityProviders { get; } = new Dictionary<string, IIdentityProvider>();
    }
}
