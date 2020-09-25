using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml2;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Configuration
{
    public class PartnerSaml2pIdentityProvider : Saml2pIdentityProvider
    {
        public string NameIdPolicyFormat { get; set; } = Saml2Constants.NameIdentifierFormats.UnspecifiedString;
        public Uri RequestedAuthnContextClassRef { get; set; } = new Uri(Saml2pConstants.Classes.Unspecified);
        public X509Certificate2 TokenSignatureVerificationCertificate { get; set; }
        public X509Certificate2 EncryptionCertificate { get; set; }
        public X509Certificate2 AuthnRequestSigningCertificate { get; set; }
        public Saml2pServiceProviderOptions ServiceProvider { get; internal set; }
    }
}
