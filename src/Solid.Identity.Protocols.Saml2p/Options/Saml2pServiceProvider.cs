using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Solid.Identity.Protocols.Saml2p.Abstractions;
using Solid.Identity.Protocols.Saml2p.Models;
using Solid.Identity.Protocols.Saml2p.Models.Context;
using Solid.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.Saml2p.Options
{
    /// <summary>
    /// The default implementation of <see cref="ISaml2pServiceProvider"/>.
    /// </summary>
    public class Saml2pServiceProvider : ISaml2pServiceProvider
    {
        /// <inheritdoc/>
        public string Id { get; set; }

        /// <inheritdoc/>
        public string ExpectedIssuer { get; set; }

        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public ICollection<BindingType> SupportedBindings { get; internal set; } = Saml2pConstants.Bindings.All;

        /// <inheritdoc/>
        public ICollection<string> RequiredClaims { get; internal set; } = new List<string>();

        /// <inheritdoc/>
        public ICollection<string> OptionalClaims { get; internal set; } = new List<string>();

        /// <inheritdoc/>
        public bool Enabled { get; set; } = true;

        /// <inheritdoc/>
        public PathString AssertionConsumerServiceEndpoint { get; set; }

        /// <inheritdoc/>
        public SecurityKey AssertionSigningKey { get; set; }

        /// <inheritdoc/>
        public TimeSpan? TokenLifeTime { get; set; }

        /// <inheritdoc/>
        public TimeSpan? MaxClockSkew { get; set; }

        /// <inheritdoc/>
        public SignatureMethod AssertionSigningMethod { get; set; } = SignatureMethod.RsaSha256;

        /// <inheritdoc/>
        public SecurityKey AssertionEncryptionKey { get; set; }

        /// <inheritdoc/>
        public EncryptionMethod AssertionEncryptionMethod { get; set; }

        /// <inheritdoc/>
        public bool RequiresEncryptedAssertion { get; set; } = false;

        /// <inheritdoc/>
        public bool RequiresSignedSamlResponse { get; set; }
        
        /// <inheritdoc/>
        public SecurityKey SamlResponseSigningKey { get; set; }
        
        /// <inheritdoc/>
        public SignatureMethod SamlResponseSigningMethod { get; set; }

        /// <inheritdoc/>
        public SecurityKey AuthnRequestSigningKey { get; set; }

        /// <inheritdoc/>
        public bool CanInitiateSso { get; set; } = true;

        /// <inheritdoc/>
        public bool AllowsIdpInitiatedSso { get; set; } = true;

        /// <inheritdoc/>
        public Uri BaseUrl { get; set; }

        /// <inheritdoc/>
        public bool AllowClaimsPassthrough { get; set; } = false;

        // You have identity provider events on a service provider becuase you are the identity provider in this scenario.
        /// <inheritdoc/>
        public Saml2pIdentityProviderEvents Events { get; } = new Saml2pIdentityProviderEvents();
    }
}
