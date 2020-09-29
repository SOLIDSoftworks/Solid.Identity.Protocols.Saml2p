using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Solid.Identity.Protocols.Saml2p.Models.Context;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.Saml2p.Options
{
    public class Saml2pIdentityProviderEvents
    {
        public Func<IServiceProvider, InitiateSsoContext, Task> OnInitiateSso { get; set; } = (provider, context) => DefaultChallengeAsync(provider, context.ReturnUrl);
        public Func<IServiceProvider, AcceptSsoContext, Task> OnAcceptSso { get; set; } = (provider, context) => DefaultChallengeAsync(provider, context.ReturnUrl);
        public Func<IServiceProvider, CompleteSsoContext, Task> OnCompleteSso { get; set; } = (_, __) => Task.CompletedTask;
        public Func<IServiceProvider, CreateSecurityTokenContext, Task> OnCreateSecurityToken { get; set; } = (_, __) => Task.CompletedTask;

        internal Task InitiateSsoAsync(IServiceProvider provider, InitiateSsoContext context) => OnInitiateSso(provider, context);
        internal Task AcceptSsoAsync(IServiceProvider provider, AcceptSsoContext context) => OnAcceptSso(provider, context);
        internal Task CompleteSsoAsync(IServiceProvider provider, CompleteSsoContext context) => OnCompleteSso(provider, context);
        internal Task CreateSecurityTokenAsync(IServiceProvider provider, CreateSecurityTokenContext context) => OnCreateSecurityToken(provider, context);

        private static async Task DefaultChallengeAsync(IServiceProvider provider, string returnUrl)
        {
            var context = provider.GetRequiredService<IHttpContextAccessor>().HttpContext;
            if (context.User.Identity.IsAuthenticated)
            {
                context.Response.Redirect(returnUrl);
                return;
            }

            var request = context.Request;
            var properties = new AuthenticationProperties
            {
                RedirectUri = returnUrl
            };
            await context.ChallengeAsync(properties);
        }
    }
}
