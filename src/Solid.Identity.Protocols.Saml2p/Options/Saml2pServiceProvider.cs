using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Solid.Identity.Protocols.Saml2p.Models.Context;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.Saml2p.Options
{
    public class Saml2pServiceProvider : ISaml2pServiceProvider
    {
        public string ExpectedIssuer { get; set; }

        public string Id { get; set; }

        public string Name { get; set; }

        public ICollection<string> SupportedBindings { get; internal set; } = Saml2pConstants.Bindings.All;
        public ICollection<string> RequiredClaims { get; internal set; } = new List<string>();
        public ICollection<string> OptionalClaims { get; internal set; } = new List<string>();

        public bool Enabled { get; set; } = true;

        public PathString AssertionConsumerServiceEndpoint { get; set; }

        public SecurityKey AssertionSigningKey { get; set; }
        public SecurityKey ResponseSigningKey { get; set; }
        public TimeSpan? TokenLifeTime { get; set; }
        public TimeSpan? MaxClockSkew { get; set; }

        public string AssertionSigningAlgorithm { get; set; } = SecurityAlgorithms.RsaSha256Signature;

        public string AssertionSigningDigestAlgorithm { get; set; } = SecurityAlgorithms.Sha256;

        public SecurityKey AssertionEncryptionKey { get; set; }

        public string AssertionEncryptionAlgorithm { get; set; } = SecurityAlgorithms.Aes128Encryption;

        public string AssertionEncryptionKeyWrapAlgorithm { get; set; } = SecurityAlgorithms.RsaOaepKeyWrap;

        public bool RequiresEncryptedAssertion { get; set; } = false;

        public bool CanInitiateSso { get; set; } = true;

        public bool AllowsIdpInitiatedSso { get; set; } = true;

        public Uri BaseUrl { get; set; }

        public bool AllowClaimsPassthrough { get; set; } = false;

        public Func<IServiceProvider, AcceptSsoContext, ValueTask> OnAcceptSso { get; set; } = (_, __) => new ValueTask();

        public Func<IServiceProvider, InitiateSsoContext, ValueTask> OnInitiateSso { get; set; } = (_, __) => new ValueTask();

        public Func<IServiceProvider, CompleteSsoContext, ValueTask> OnCompleteSso { get; set; } = (_, __) => new ValueTask();

        public Func<IServiceProvider, CreateSecurityTokenContext, ValueTask> OnCreatingSecurityToken { get; set; } = (_, __) => new ValueTask();

        public Func<IServiceProvider, CreateSecurityTokenContext, ValueTask> OnCreatedSecurityToken { get; set; } = (_, __) => new ValueTask();
    }

    public interface ISaml2pServiceProvider : ISaml2pPartner
    {
        string ExpectedIssuer { get; }
        string Id { get; }
        string Name { get; }
        bool Enabled { get; }
        PathString AssertionConsumerServiceEndpoint { get; }
        SecurityKey AssertionSigningKey { get; }
        string AssertionSigningAlgorithm { get; }
        string AssertionSigningDigestAlgorithm { get; }
        //SecurityKey AssertionEncryptionKey { get; }
        //string AssertionEncryptionAlgorithm { get; }
        //string AssertionEncryptionKeyWrapAlgorithm { get; }
        //SecurityKey ResponseSigningKey { get; }
        //string ResponseSigningAlgorithm { get; }
        //string ResponseDigestAlgorithm { get; }
        TimeSpan? TokenLifeTime { get; }
        TimeSpan? MaxClockSkew { get; }
        ICollection<string> RequiredClaims { get; }
        ICollection<string> OptionalClaims { get; }
        //bool RequiresEncryptedAssertion { get; }
        bool AllowsIdpInitiatedSso { get; }

        bool AllowClaimsPassthrough {  get; }

        Func<IServiceProvider, AcceptSsoContext, ValueTask> OnAcceptSso { get; }

        Func<IServiceProvider, InitiateSsoContext, ValueTask> OnInitiateSso { get; }

        Func<IServiceProvider, CompleteSsoContext, ValueTask> OnCompleteSso { get; }
        Func<IServiceProvider, CreateSecurityTokenContext, ValueTask> OnCreatingSecurityToken { get; }
        Func<IServiceProvider, CreateSecurityTokenContext, ValueTask> OnCreatedSecurityToken { get; }
    }
}
