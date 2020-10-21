using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Solid.Identity.Protocols.Saml2p.Models.Context;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.Saml2p.Abstractions
{
    public interface ISaml2pIdentityProvider : ISaml2pPartner
    {
        string NameIdPolicyFormat { get; }
        Uri RequestedAuthnContextClassRef { get; }
        PathString AcceptSsoEndpoint { get; }
        ICollection<SecurityKey> AssertionSigningKeys { get; }
        Func<IServiceProvider, StartSsoContext, ValueTask> OnStartSso { get; }
        Func<IServiceProvider, FinishSsoContext, ValueTask> OnFinishSso { get; }
        Func<IServiceProvider, ValidateTokenContext, ValueTask> OnValidatingToken { get; }
        Func<IServiceProvider, ValidateTokenContext, ValueTask> OnValidatedToken { get; }
        Func<IServiceProvider, GenerateRelayStateContext, ValueTask> OnGeneratingRelayState { get; }
    }
}
