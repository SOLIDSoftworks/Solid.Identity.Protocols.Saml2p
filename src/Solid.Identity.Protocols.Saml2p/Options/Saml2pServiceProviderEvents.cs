using Solid.Identity.Protocols.Saml2p.Models.Context;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.Saml2p.Options
{
    /// <summary>
    /// Events that are run during SP flows.
    /// </summary>
    public class Saml2pServiceProviderEvents
    {
        /// <summary>
        /// This is run just before an <see cref="AuthnRequest"/> is sent to an IDP.
        /// </summary>
        public Func<IServiceProvider, StartSsoContext, ValueTask> OnStartSso { get; set; } = (_, __) => new ValueTask();

        /// <summary>
        /// This is run when a <see cref="SamlResponse"/> has been received from an IDP.
        /// <para>This is run before <see cref="OnValidatingToken"/> and <see cref="OnValidatedToken"/>.</para>
        /// </summary>
        public Func<IServiceProvider, FinishSsoContext, ValueTask> OnFinishSso { get; set; } = (_, __) => new ValueTask();

        /// <summary>
        /// This is run just before an incoming token is validated.
        /// <para>You can manually populate <see cref="ValidateTokenContext.SecurityToken"/> and <see cref="ValidateTokenContext.Subject"/> here, but they must both be popluated.</para>
        /// </summary>
        public Func<IServiceProvider, ValidateTokenContext, ValueTask> OnValidatingToken { get; set; } = (_, __) => new ValueTask();

        /// <summary>
        /// This is run just after an incoming token has been validated.
        /// </summary>
        public Func<IServiceProvider, ValidateTokenContext, ValueTask> OnValidatedToken { get; set; } = (_, __) => new ValueTask();

        /// <summary>
        /// This is run just before the outgoing relay state is generated.
        /// <para>You can manually populate <see cref="GenerateRelayStateContext.RelayState"/> here.</para>
        /// </summary>
        public Func<IServiceProvider, GenerateRelayStateContext, ValueTask> OnGeneratingRelayState { get; set; } = (_, __) => new ValueTask();
    }
}
