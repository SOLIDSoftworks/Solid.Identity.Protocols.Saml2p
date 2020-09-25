using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Abstractions.Configuration
{
    public interface IPartnerServiceProvider
    {
        string EntityId { get; }
        string Audience { get; }
        Uri BaseUrl { get; }
        IBinding AssertionConsumerService { get; }
        TimeSpan? ClockSkewTolerence { get; }
        bool SignAssertion { get; }
        SecurityKey SigningKey { get; }
        string SigningAlgorithm { get; }
        string SigningDigestAlgorithm { get; }
        
        SecurityKey EncryptionKey { get; }
        string EncryptionAlgorithm { get; }
        string EncryptionKeyWrapAlgorithm { get; }
        bool EncryptAssertion { get; set; }
        IIdentityProviderConfiguration IdentityProvider { get; }
    }
}
