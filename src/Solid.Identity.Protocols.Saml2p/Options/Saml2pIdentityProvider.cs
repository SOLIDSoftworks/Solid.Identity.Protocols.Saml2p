using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml2;
using Solid.Identity.Protocols.Saml2p.Abstractions;
using Solid.Identity.Protocols.Saml2p.Models;
using Solid.Identity.Protocols.Saml2p.Models.Context;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Solid.IdentityModel.Tokens;

namespace Solid.Identity.Protocols.Saml2p.Options
{
    /// <summary>
    /// The default implementation of <see cref="ISaml2pIdentityProvider"/>.
    /// </summary>
    public class Saml2pIdentityProvider : ISaml2pIdentityProvider
    {
        /// <inheritdoc/>
        public string Id { get; set; }

        /// <inheritdoc/>
        public string ExpectedIssuer { get; set; }

        /// <inheritdoc/>
        public string Name { get; set; }

        /// <inheritdoc/>
        public string NameIdPolicyFormat { get; set; } = Saml2Constants.NameIdentifierFormats.UnspecifiedString;

        /// <inheritdoc/>
        public Uri RequestedAuthnContextClassRef { get; set; } = Saml2pConstants.Classes.Unspecified;

        /// <inheritdoc/>
        public Comparison RequestedAuthnContextClassRefComparison { get; set; } = Comparison.Exact;

        /// <inheritdoc/>
        public PathString AcceptSsoEndpoint { get; set; }

        /// <inheritdoc/>
        public ICollection<SecurityKey> AssertionSigningKeys { get; internal set; } = new List<SecurityKey>();

        /// <inheritdoc/>
        public ICollection<SecurityKey> AssertionDecryptionKeys { get; internal set; } = new List<SecurityKey>();

        /// <inheritdoc/>
        public Uri BaseUrl { get; set; }

        /// <inheritdoc/>
        public bool Enabled { get; set; } = true;

        /// <inheritdoc/>
        public ICollection<BindingType> SupportedBindings { get; internal set; } = Saml2pConstants.Bindings.All;

        /// <inheritdoc/>
        public bool CanInitiateSso { get; set; } = true;

        /// <inheritdoc/>
        public bool AllowsSpInitiatedSso { get; set; } = true;
        
        /// <inheritdoc/>
        public bool RequiresSignedAuthnRequest { get; set; }
        
        /// <inheritdoc/>
        public SignatureMethod AuthnRequestSigningMethod { get; set; }

        // You have service provider events on a identity provider becuase you are the service provider in this scenario.
        /// <inheritdoc/>
        public Saml2pServiceProviderEvents Events { get; } = new Saml2pServiceProviderEvents();
        
        /// <inheritdoc/>
        public SecurityKey SamlResponseSigningKey { get; set; }
        
        /// <inheritdoc/>
        public SecurityKey AuthnRequestSigningKey { get; set; }
    }
}
