using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Configuration
{
    public class PartnerSaml2pServiceProvider : Saml2pServiceProvider
    {
        public SecurityKey SigningKey { get; set; }
        public string SigningAlgorithm { get; set; } = SecurityAlgorithms.RsaSha256Signature;
        public string SigningDigestAlgorithm { get; set; } = SecurityAlgorithms.Sha256;

        public bool EncryptAssertion { get; set; } = false;
        public SecurityKey EncryptionKey { get; set; }
        public string EncryptionAlgorithm { get; set; } = SecurityAlgorithms.Aes128Encryption;
        public string EncryptionKeyWrapAlgorithm { get; set; } = SecurityAlgorithms.RsaOaepKeyWrap;

        public SecurityKey AuthnRequestSignatureVerificationKey { get; set; }
        public Saml2pIdentityProviderOptions IdentityProvider { get; internal set; }
    }
}
