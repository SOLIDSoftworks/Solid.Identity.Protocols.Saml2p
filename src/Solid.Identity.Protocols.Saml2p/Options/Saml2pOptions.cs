using Microsoft.AspNetCore.Http;
using Solid.Identity.Protocols.Saml2p.Abstractions;
using Solid.Identity.Protocols.Saml2p.Models;
using Solid.Identity.Protocols.Saml2p.Models.Context;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.Saml2p.Options
{
    /// <summary>
    /// Options for configuring Saml2p.
    /// </summary>
    public class Saml2pOptions
    {
        /// <summary>
        /// The default issuer id used when creating <see cref="AuthnRequest"/> and <see cref="SamlResponse"/>.
        /// </summary>
        public string DefaultIssuer { get; set; }

        /// <summary>
        /// The default <see cref="TimeSpan"/> that is allowed for clock skew.
        /// </summary>
        public TimeSpan? DefaultMaxClockSkew { get; set; }

        /// <summary>
        /// The default lifetime of an issued security token.
        /// </summary>
        public TimeSpan DefaultTokenLifetime { get; set; } = TimeSpan.FromMinutes(5);

        /// <summary>
        /// The query parameter read from the url that should contain a partner identifier.
        /// </summary>
        public string PartnerIdQueryParameter { get; set; } = "partnerId";

        /// <summary>
        /// The supported Saml2p bindings.
        /// </summary>
        public ICollection<BindingType> SupportedBindings { get; internal set; } = Saml2pConstants.Bindings.All;

        /// <summary>
        /// The relative path used for accepting <see cref="AuthnRequest"/>s (IDP flow).
        /// </summary>
        public PathString AcceptPath { get; set; } = PathString.Empty;

        /// <summary>
        /// The relative path used for IDP initiated SSO (IDP flow).
        /// </summary>
        public PathString InitiatePath { get; set; } = "/initiate";

        /// <summary>
        /// The relative path used to complete SSO (IDP flow).
        /// </summary>
        public PathString CompletePath { get; set; } = "/complete";

        /// <summary>
        /// The relative path used to start SSO (SP flow).
        /// </summary>
        public PathString StartPath { get; set; } = "/start";

        /// <summary>
        /// The relative path used to finish SSO (SP flow).
        /// </summary>
        public PathString FinishPath { get; set; } = "/finish";

        /// <summary>
        /// The relative path used by default after SSO has finished (SP flow).
        /// </summary>
        public PathString DefaultRedirectPath { get; set; } = "/";

        /// <summary>
        /// Events object that contains delegates to be run during SSO (IDP flow).
        /// </summary>
        public Saml2pIdentityProviderEvents IdentityProviderEvents { get; } = new Saml2pIdentityProviderEvents();

        /// <summary>
        /// Events object that contains delegates to be run during SSO (SP flow).
        /// </summary>
        public Saml2pServiceProviderEvents ServiceProviderEvents { get; } = new Saml2pServiceProviderEvents();

        /// <summary>
        /// Adds a service provider partner.
        /// </summary>
        /// <param name="id">The id of the service provider.</param>
        /// <param name="configure">A delegate that configures a <see cref="Saml2pServiceProvider"/> instance.</param>
        /// <returns>This <see cref="Saml2pOptions"/> instance so that additional calls can be chained.</returns>
        public Saml2pOptions AddServiceProvider(string id, Action<Saml2pServiceProvider> configure)
        {
            var sp = new Saml2pServiceProvider { Id = id, Name = id };
            configure(sp);
            ServiceProviders[id] = sp;
            return this;
        }

        /// <summary>
        /// Adds an identity provider partner.
        /// </summary>
        /// <param name="id">The id of the identity provider.</param>
        /// <param name="configure">A delegate that configures a <see cref="Saml2pIdentityProvider"/> instance.</param>
        /// <returns>This <see cref="Saml2pOptions"/> instance so that additional calls can be chained.</returns>
        public Saml2pOptions AddIdentityProvider(string id, Action<Saml2pIdentityProvider> configure)
        {
            var sp = new Saml2pIdentityProvider { Id = id, Name = id };
            configure(sp);
            IdentityProviders[id] = sp;
            return this;
        }

        internal IDictionary<string, ISaml2pServiceProvider> ServiceProviders { get; } = new Dictionary<string, ISaml2pServiceProvider>(StringComparer.OrdinalIgnoreCase);

        internal IDictionary<string, ISaml2pIdentityProvider> IdentityProviders { get; } = new Dictionary<string, ISaml2pIdentityProvider>(StringComparer.OrdinalIgnoreCase);
    }
}
