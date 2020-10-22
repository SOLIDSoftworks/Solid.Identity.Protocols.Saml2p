using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Solid.Identity.Protocols.Saml2p.Models.Context;
using Solid.Identity.Protocols.Saml2p.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.Saml2p.Abstractions
{
    public interface ISaml2pServiceProvider : ISaml2pPartner<Saml2pIdentityProviderEvents>
    {
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

        bool AllowClaimsPassthrough { get; }
    }
}
