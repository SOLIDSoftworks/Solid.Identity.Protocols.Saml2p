using Solid.Identity.Protocols.Saml2p.Models.Context;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.Saml2p.Options
{
    public class Saml2pServiceProviderEvents
    {
        public Func<IServiceProvider, StartSsoContext, ValueTask> OnStartSso { get; set; } = (_, __) => new ValueTask();

        public Func<IServiceProvider, FinishSsoContext, ValueTask> OnFinishSso { get; set; } = (_, __) => new ValueTask();

        public Func<IServiceProvider, ValidateTokenContext, ValueTask> OnValidatingToken { get; set; } = (_, __) => new ValueTask();

        public Func<IServiceProvider, ValidateTokenContext, ValueTask> OnValidatedToken { get; set; } = (_, __) => new ValueTask();

        public Func<IServiceProvider, GenerateRelayStateContext, ValueTask> OnGeneratingRelayState { get; set; } = (_, __) => new ValueTask();
    }
}
