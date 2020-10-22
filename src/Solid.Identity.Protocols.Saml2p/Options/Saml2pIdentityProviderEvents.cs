using Solid.Identity.Protocols.Saml2p.Models.Context;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.Saml2p.Options
{
    public class Saml2pIdentityProviderEvents
    {
        public Func<IServiceProvider, AcceptSsoContext, ValueTask> OnAcceptSso { get; set; } = (_, __) => new ValueTask();

        public Func<IServiceProvider, InitiateSsoContext, ValueTask> OnInitiateSso { get; set; } = (_, __) => new ValueTask();

        public Func<IServiceProvider, CompleteSsoContext, ValueTask> OnCompleteSso { get; set; } = (_, __) => new ValueTask();

        public Func<IServiceProvider, CreateSecurityTokenContext, ValueTask> OnCreatingSecurityToken { get; set; } = (_, __) => new ValueTask();

        public Func<IServiceProvider, CreateSecurityTokenContext, ValueTask> OnCreatedSecurityToken { get; set; } = (_, __) => new ValueTask();
    }
}
