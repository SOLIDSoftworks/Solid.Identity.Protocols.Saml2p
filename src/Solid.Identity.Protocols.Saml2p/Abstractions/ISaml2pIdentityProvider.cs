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
    /// <summary>
    /// An interface describing an IDP partner. 
    /// </summary>
    public interface ISaml2pIdentityProvider : ISaml2pPartner<Saml2pServiceProviderEvents>
    {
        /// <summary>
        /// The subject name id format requested from the IDP partner.
        /// </summary>
        string NameIdPolicyFormat { get; }

        /// <summary>
        /// The requested authentication type from the IDP partner.
        /// </summary>
        Uri RequestedAuthnContextClassRef { get; }

        /// <summary>
        /// The endpoint on the IDP partner service that is used when starting SSO.
        /// </summary>
        PathString AcceptSsoEndpoint { get; }

        /// <summary>
        /// A collection of <see cref="SecurityKey"/>s that are used to validate the SAML2 security token coming from the IDP partner.
        /// </summary>
        ICollection<SecurityKey> AssertionSigningKeys { get; }

        /// <summary>
        /// A flag indicating whether the SP can initiate SSO.
        /// </summary>
        bool AllowsSpInitiatedSso { get; }
    }
}
