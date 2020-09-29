using Solid.Identity.Protocols.Saml2p.Providers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Solid.Identity.Protocols.Saml2p.Abstractions.Services;
using Solid.Identity.Protocols.Saml2p.Serialization;
using System.Security.Claims;
using Solid.Identity.Protocols.Saml2p.Options;

namespace Microsoft.AspNetCore.Http
{
    public static class Solid_Identity_Protocols_Saml2p_HttpContextExtensions
    {
        // Accept AuthnRequest (AcceptSso)
        // Accept SamlResponse (CompleteSso)
        // Send AuthnRequest (StartSso)
        // Send SamlResponse (InitiateSso)

        public static bool CanAcceptSsoAs(this HttpContext context, string idpId)
        {
            var service = context.RequestServices.GetRequiredService<Saml2pOptionsProvider>();
            var idp = service.GetIdentityProviderOptions(idpId);
            if (idp == null) return false;
            return context.CanAcceptSsoAs(idp);
        }
        public static bool CanAcceptSsoAs(this HttpContext context, Saml2pIdentityProviderOptions idp)
        {
            // TODO: log
            if (!idp.Enabled) return false;
            if (!HttpMethods.IsPost(context.Request.Method)) return false;
            var prefixes = context.RequestServices.GetRequiredService<PathPrefixProvider>();
            return prefixes.GetPrefix(idp.Id).Add(idp.SsoEndpoint) == context.Request.Path;
        }

        //public static async Task AcceptSsoAsync(this HttpContext context)
        //{
        //    var idp = context.RequestServices.GetRequiredService<ISaml2pIdentityProviderService>();
        //    await idp.AcceptSsoAsync();
        //}

        public static async Task AcceptSsoAsAsync(this HttpContext context, Saml2pIdentityProviderOptions idp)
        {
            var service = context.RequestServices.GetRequiredService<ISaml2pIdentityProviderService>();
            await service.AcceptSsoAsAsync(idp);
        }

        public static bool CanInitiateSsoAs(this HttpContext context, string idpId)
        {
            var service = context.RequestServices.GetRequiredService<Saml2pOptionsProvider>();
            var idp = service.GetIdentityProviderOptions(idpId);
            if (idp == null) return false;
            return context.CanInitiateSsoAs(idp);
        }

        public static bool CanInitiateSsoAs(this HttpContext context, Saml2pIdentityProviderOptions idp)
        {
            // TODO: log
            if (!idp.Enabled) return false;
            if (!HttpMethods.IsGet(context.Request.Method)) return false;
            var prefixes = context.RequestServices.GetRequiredService<PathPrefixProvider>();
            return prefixes.GetPrefix(idp.Id).Add(idp.SsoEndpoint).Add("/initiate") == context.Request.Path;

        }

        //public static async Task InitiateSsoAsync(this HttpContext context)
        //{
        //    var idp = context.RequestServices.GetRequiredService<ISaml2pIdentityProviderService>();
        //    await idp.InitiateSsoAsync();
        //}

        public static async Task InitiateSsoAsAsync(this HttpContext context, Saml2pIdentityProviderOptions idp)
        {
            var service = context.RequestServices.GetRequiredService<ISaml2pIdentityProviderService>();
            await service.InitiateSsoAsAsync(idp);
        }

        public static bool CanCompleteSsoAs(this HttpContext context, Saml2pIdentityProviderOptions idp)
        {
            // TODO: log
            if (!idp.Enabled) return false;
            if (!HttpMethods.IsGet(context.Request.Method)) return false;
            var prefixes = context.RequestServices.GetRequiredService<PathPrefixProvider>();
            return prefixes.GetPrefix(idp.Id).Add(idp.SsoEndpoint).Add("/complete") == context.Request.Path;
        }

        //public static async Task CompleteSsoAsync(this HttpContext context)
        //{
        //    var idp = context.RequestServices.GetRequiredService<ISaml2pIdentityProviderService>();
        //    await idp.CompleteSsoAsync();
        //}

        public static async Task CompleteSsoAsAsync(this HttpContext context, Saml2pIdentityProviderOptions idp)
        {
            var service = context.RequestServices.GetRequiredService<ISaml2pIdentityProviderService>();
            await service.CompleteSsoAsAsync(idp);
        }

        public static Task StartSsoAsync(this HttpContext context, string partnerId)
        {
            var spService = context.RequestServices.GetRequiredService<ISaml2pServiceProviderService>();
            return spService.StartSsoAsync(partnerId);
        }

        public static Task<ClaimsPrincipal> FinishSsoAsync(this HttpContext context)
        {
            var spService = context.RequestServices.GetRequiredService<ISaml2pServiceProviderService>();
            return spService.FinishSsoAsync();
        }
    }
}
