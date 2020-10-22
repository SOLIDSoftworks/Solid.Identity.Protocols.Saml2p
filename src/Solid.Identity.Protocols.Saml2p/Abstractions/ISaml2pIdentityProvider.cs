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
    public interface ISaml2pIdentityProvider : ISaml2pPartner<Saml2pServiceProviderEvents>
    {
        string NameIdPolicyFormat { get; }
        Uri RequestedAuthnContextClassRef { get; }
        PathString AcceptSsoEndpoint { get; }
        ICollection<SecurityKey> AssertionSigningKeys { get; }
    }
}
