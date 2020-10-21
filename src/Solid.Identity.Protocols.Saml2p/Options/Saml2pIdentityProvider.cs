using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.Tokens.Saml2;
using Solid.Identity.Protocols.Saml2p.Models.Context;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.Saml2p.Options
{
    public class Saml2pIdentityProvider : ISaml2pIdentityProvider
    {
        public string Id { get; set; }

        public string ExpectedIssuer { get; set; }

        public string Name { get; set; }

        public string NameIdPolicyFormat { get; set; } = Saml2Constants.NameIdentifierFormats.UnspecifiedString;

        public Uri RequestedAuthnContextClassRef { get; set; } = Saml2pConstants.Classes.Unspecified;

        public PathString SsoEndpoint { get; set; }

        public ICollection<SecurityKey> AssertionSigningKeys { get; internal set; } = new List<SecurityKey>();

        public Uri BaseUrl { get; set; }

        public bool Enabled { get; set; } = true;

        public Func<IServiceProvider, StartSsoContext, ValueTask> OnStartSso { get; set; } = (_, __) => new ValueTask();

        public Func<IServiceProvider, FinishSsoContext, ValueTask> OnFinishSso { get; set; } = (_, __) => new ValueTask();

        public Func<IServiceProvider, ValidateTokenContext, ValueTask> OnValidatingToken { get; set; } = (_, __) => new ValueTask();

        public Func<IServiceProvider, ValidateTokenContext, ValueTask> OnValidatedToken { get; set; } = (_, __) => new ValueTask();
        public Func<IServiceProvider, GenerateRelayStateContext, ValueTask> OnGeneratingRelayState { get; set; } = (_, __) => new ValueTask();

        public ICollection<string> SupportedBindings { get; internal set; } = Saml2pConstants.Bindings.All;

        public bool CanInitiateSso { get; set; } = true;
    }

    public interface ISaml2pIdentityProvider : ISaml2pPartner
    { 
        string NameIdPolicyFormat { get; }
        Uri RequestedAuthnContextClassRef { get; }
        PathString SsoEndpoint { get; }

        ICollection<SecurityKey> AssertionSigningKeys { get; }
        Func<IServiceProvider, StartSsoContext, ValueTask> OnStartSso { get; }
        Func<IServiceProvider, FinishSsoContext, ValueTask> OnFinishSso { get; }
        Func<IServiceProvider, ValidateTokenContext, ValueTask> OnValidatingToken { get; }
        Func<IServiceProvider, ValidateTokenContext, ValueTask> OnValidatedToken { get; }
        Func<IServiceProvider, GenerateRelayStateContext, ValueTask> OnGeneratingRelayState { get; }
    }
}
