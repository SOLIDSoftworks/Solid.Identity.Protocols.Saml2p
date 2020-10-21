//using Microsoft.AspNetCore.Authentication;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.IdentityModel.Tokens.Saml2;
//using Solid.Identity.Protocols.Saml2p.Models.Context;
//using Solid.Identity.Protocols.Saml2p.Models.Protocol;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;

//namespace Solid.Identity.Protocols.Saml2p.Options
//{
//    public class Saml2pIdentityProviderEvents
//    {
//        public Func<IServiceProvider, InitiateSsoContext, ValueTask> InitiateSso { get; set; } = (provider, context) => DefaultChallengeAsync(provider, context.ReturnUrl);
//        public Func<IServiceProvider, AcceptSsoContext, ValueTask> AcceptSso { get; set; } = (provider, context) => DefaultChallengeAsync(provider, context.ReturnUrl);
//        public Func<IServiceProvider, CompleteSsoContext, ValueTask> CompleteSso { get; set; } = (_, __) => new ValueTask();
//        public Func<IServiceProvider, CreateSecurityTokenContext, ValueTask<Saml2SecurityToken>> CreateSecurityToken { get; set; } = (provider, context) => DefaultCreateSecurityTokenAsync(provider, context);

//        internal ValueTask InitiateSsoAsync(IServiceProvider provider, InitiateSsoContext context) => InitiateSso(provider, context);
//        internal ValueTask AcceptSsoAsync(IServiceProvider provider, AcceptSsoContext context) => AcceptSso(provider, context);
//        internal ValueTask CompleteSsoAsync(IServiceProvider provider, CompleteSsoContext context) => CompleteSso(provider, context);
//        internal ValueTask<Saml2SecurityToken> CreateSecurityTokenAsync(IServiceProvider provider, CreateSecurityTokenContext context) => CreateSecurityToken(provider, context);

//        private static async ValueTask DefaultChallengeAsync(IServiceProvider provider, string returnUrl)
//        {
//            var context = provider.GetRequiredService<IHttpContextAccessor>().HttpContext;
//            if (context.User.Identity.IsAuthenticated)
//            {
//                context.Response.Redirect(returnUrl);
//                return;
//            }

//            var request = context.Request;
//            var properties = new AuthenticationProperties
//            {
//                RedirectUri = returnUrl
//            };
//            await context.ChallengeAsync(properties);
//        }

//        private static ValueTask<Saml2SecurityToken> DefaultCreateSecurityTokenAsync(IServiceProvider provider, CreateSecurityTokenContext context)
//        {
//            var token = context.Handler.CreateToken(context.TokenDescriptor) as Saml2SecurityToken;
//            return new ValueTask<Saml2SecurityToken>(token as Saml2SecurityToken);
//        }
//    }
//}
