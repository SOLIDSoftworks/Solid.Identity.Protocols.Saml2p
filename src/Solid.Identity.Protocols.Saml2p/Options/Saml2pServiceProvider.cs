using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Solid.Identity.Protocols.Saml2p.Abstractions;
using Solid.Identity.Protocols.Saml2p.Models;
using Solid.Identity.Protocols.Saml2p.Models.Context;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.Saml2p.Options
{
    public class Saml2pServiceProvider : ISaml2pServiceProvider
    {
        public string ExpectedIssuer { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public ICollection<BindingType> SupportedBindings { get; internal set; } = Saml2pConstants.Bindings.All;
        public ICollection<string> RequiredClaims { get; internal set; } = new List<string>();
        public ICollection<string> OptionalClaims { get; internal set; } = new List<string>();

        public bool Enabled { get; set; } = true;

        public PathString AssertionConsumerServiceEndpoint { get; set; }

        public SecurityKey AssertionSigningKey { get; set; }
        public SecurityKey ResponseSigningKey { get; set; }
        public TimeSpan? TokenLifeTime { get; set; }
        public TimeSpan? MaxClockSkew { get; set; }

        public string AssertionSigningAlgorithm { get; set; } = SecurityAlgorithms.RsaSha256Signature;

        public string AssertionSigningDigestAlgorithm { get; set; } = SecurityAlgorithms.Sha256Digest;

        public SecurityKey AssertionEncryptionKey { get; set; }

        public string AssertionEncryptionAlgorithm { get; set; } = SecurityAlgorithms.Aes128Encryption;

        public string AssertionEncryptionKeyWrapAlgorithm { get; set; } = SecurityAlgorithms.RsaOaepKeyWrap;

        public bool RequiresEncryptedAssertion { get; set; } = false;


        public bool CanInitiateSso { get; set; } = true;

        public bool AllowsIdpInitiatedSso { get; set; } = true;

        public Uri BaseUrl { get; set; }

        public bool AllowClaimsPassthrough { get; set; } = false;

        // You have identity provider events on a service provider becuase you are the identity provider in this scenario.
        public Saml2pIdentityProviderEvents Events { get; } = new Saml2pIdentityProviderEvents();
    }
}
