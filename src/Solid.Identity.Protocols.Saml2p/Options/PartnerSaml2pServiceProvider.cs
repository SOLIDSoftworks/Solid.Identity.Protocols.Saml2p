using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Options
{
    public class PartnerSaml2pServiceProvider : Saml2pServiceProvider
    {
        public Uri BaseUrl { get; set; }
        public SecurityKey AssertionSigningKey { get; set; }
        public string AssertionSigningAlgorithm { get; set; } = SecurityAlgorithms.RsaSha256Signature;
        public string AssertionSigningDigestAlgorithm { get; set; } = SecurityAlgorithms.Sha256;

        public bool EncryptAssertion { get; set; } = false;
        public SecurityKey AssertionEncryptionKey { get; set; }
        public string AssertionEncryptionAlgorithm { get; set; } = SecurityAlgorithms.Aes128Encryption;
        public string AssertionEncryptionKeyWrapAlgorithm { get; set; } = SecurityAlgorithms.RsaOaepKeyWrap;

        public SecurityKey AuthnRequestSignatureVerificationKey { get; set; }
        public Saml2pIdentityProviderOptions IdentityProvider { get; internal set; }
    }
}
