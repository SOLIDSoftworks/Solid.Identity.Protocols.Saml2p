//using Microsoft.IdentityModel.Tokens.Saml2;
//using Solid.Identity.Protocols.Saml2p.Models.Context;
//using Solid.Identity.Protocols.Saml2p.Models.Protocol;
//using System;
//using System.Collections.Generic;
//using System.Security.Claims;
//using System.Text;
//using System.Threading.Tasks;

//namespace Solid.Identity.Protocols.Saml2p.Options
//{
//    public class Saml2pServiceProviderEvents
//    {
//        public Func<IServiceProvider, StartSsoContext, ValueTask> StartSso { get; set; } = (_, __) => new ValueTask();
//        public Func<IServiceProvider, FinishSsoContext, ValueTask> FinishSso { get; set; } = (_, __) => new ValueTask();
//        public Func<IServiceProvider, ValidateTokenContext, ValueTask<ClaimsPrincipal>> ValidateToken { get; set; } = (provider, context) => DefaultValidateTokenAsync(provider, context);
//        internal ValueTask StartSsoAsync(IServiceProvider provider, StartSsoContext context) => StartSso(provider, context);
//        internal ValueTask FinishSsoAsync(IServiceProvider provider, FinishSsoContext context) => FinishSso(provider, context);
//        internal ValueTask<ClaimsPrincipal> ValidateTokenAsync(IServiceProvider provider, ValidateTokenContext context) => ValidateToken(provider, context);
               
//        private static ValueTask<ClaimsPrincipal> DefaultValidateTokenAsync(IServiceProvider provider, ValidateTokenContext context)
//        {
//            var subject = context.Handler.ValidateToken(context.Response.XmlSecurityToken, context.TokenValidationParameters, out _);
//            return new ValueTask<ClaimsPrincipal>(subject);
//        }
//    }
//}
