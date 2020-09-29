using Microsoft.IdentityModel.Tokens.Saml2;
using Solid.Identity.Protocols.Saml2p.Models.Context;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.Saml2p.Options
{
    public class Saml2pServiceProviderEvents
    {
        public Func<IServiceProvider, StartSsoContext, Task> OnStartSso { get; set; } = (_, __) => Task.CompletedTask;
        public Func<IServiceProvider, FinishSsoContext, Task> OnFinishSso { get; set; } = (_, __) => Task.CompletedTask;
        public Func<IServiceProvider, TokenValidatedContext, Task> OnTokenValidated { get; set; } = (_, __) => Task.CompletedTask;

        internal Task StartSsoAsync(IServiceProvider provider, StartSsoContext context) => OnStartSso(provider, context);
        internal Task FinishSsoAsync(IServiceProvider provider, FinishSsoContext context) => OnFinishSso(provider, context);
        internal Task TokenValidatedAsync(IServiceProvider provider, TokenValidatedContext context) => OnTokenValidated(provider, context);
    }
}
