using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml2;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Options
{
    public class PartnerSaml2pIdentityProvider : Saml2pIdentityProvider
    {
        public Uri BaseUrl { get; set; }
        public PathString SsoEndpoint { get; set; }

        public string NameIdPolicyFormat { get; set; } = Saml2Constants.NameIdentifierFormats.UnspecifiedString;

        public Uri RequestedAuthnContextClassRef { get; set; } = new Uri(Saml2pConstants.Classes.Unspecified);

        public SecurityKey AssertionSigningKey { get; set; }
        
        public SecurityKey AssertionEncryptionKey { get; set; }

        public SecurityKey AuthnRequestSigningKey { get; set; }

        public Saml2pServiceProviderOptions ServiceProvider { get; internal set; }
    }
}
