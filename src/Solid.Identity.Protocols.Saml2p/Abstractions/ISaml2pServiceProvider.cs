using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Solid.Identity.Protocols.Saml2p.Models.Context;
using Solid.Identity.Protocols.Saml2p.Options;
using Solid.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.Saml2p.Abstractions
{
    /// <summary>
    /// An interface describing an SP partner. 
    /// </summary>
    public interface ISaml2pServiceProvider : ISaml2pPartner<Saml2pIdentityProviderEvents>
    {
        /// <summary>
        /// The endpoint on the SP partner service that is used when completing SSO.
        /// </summary>
        PathString AssertionConsumerServiceEndpoint { get; }

        /// <summary>
        /// The <see cref="SecurityKey"/> used to sign the Saml2 assertion.
        /// </summary>
        SecurityKey AssertionSigningKey { get; }

        /// <summary>
        /// The <see cref="SignatureMethod"/> used to sign the Saml2 assertion.
        /// </summary>
        SignatureMethod AssertionSigningMethod { get; }

        /// <summary>
        /// The lifetime of the created Saml2 assertion.
        /// </summary>
        TimeSpan? TokenLifeTime { get; }

        /// <summary>
        /// The max clock skew used when creating a  Saml2 assertion.
        /// </summary>
        TimeSpan? MaxClockSkew { get; }

        /// <summary>
        /// A collection of claims types that are required by the SP.
        /// </summary>
        ICollection<string> RequiredClaims { get; }

        /// <summary>
        /// A collection of claims types that the SP optionally supports.
        /// </summary>
        ICollection<string> OptionalClaims { get; }

        /// <summary>
        /// A flag indicating whether the IDP can initiate SSO.
        /// </summary>
        bool AllowsIdpInitiatedSso { get; }

        /// <summary>
        /// A flag indicating whether claims from the original authentication context should pass through to the SP.
        /// </summary>
        bool AllowClaimsPassthrough { get; }

        /// <summary>
        /// The <see cref="SecurityKey"/> used to encrypt the Saml2 assertion.
        /// </summary>
        SecurityKey AssertionEncryptionKey { get; }

        /// <summary>
        /// The <see cref="EncryptionMethod"/> used to encrypt the Saml2 assertion.
        /// </summary>
        EncryptionMethod AssertionEncryptionMethod { get; }

        /// <summary>
        /// A flag indicating whether encryption is required.
        /// </summary>
        bool RequiresEncryptedAssertion { get; }

        /// <summary>
        /// A flag indicating whether signing SAMLResponse is required.
        /// </summary>
        bool RequiresSignedSamlResponse { get; }
        
        /// <summary>
        /// The <see cref="SignatureMethod"/> used to sign the SAMLResponse.
        /// </summary>
        SignatureMethod SamlResponseSigningMethod { get; }
    }
}
