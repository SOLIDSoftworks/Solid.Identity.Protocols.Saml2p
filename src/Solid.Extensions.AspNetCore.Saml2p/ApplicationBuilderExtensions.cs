using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Solid.Identity.AspNetCore.Saml2p.Middleware;
using Solid.Identity.Protocols.Saml2p.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Solid.Identity.Protocols.Saml2p.Providers;

namespace Microsoft.AspNetCore.Builder
{
    public static class Solid_Identity_AspNetCore_Saml2p_ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSaml2pIdentityProvider(this IApplicationBuilder builder)
        {
            //var options = builder.ApplicationServices.GetService<IOptionsSnapshot<LocalSaml2pIdentityProvider>>();
            //var idp = options.Get(id);
            //// TODO: exception if doesn't exist
            //var path = idp.SsoEndpoint.IsAbsoluteUri ? idp.SsoEndpoint.AbsolutePath : idp.SsoEndpoint.OriginalString;
            //if (!path.StartsWith("/"))
            //    path = "/";
            //var pathMatch = new PathString(path);

            return builder
                .MapWhen(CanAcceptSso, b => b.Use((context, _) => context.AcceptSsoAsync()))
                .MapWhen(CanInitiateSso, b => b.Use((context, _) => context.InitiateSsoAsync()))
                .MapWhen(CanCompleteSso, b => b.Use((context, _) => context.CompleteSsoAsync()))
            ;
        }

        private static bool CanAcceptSso(HttpContext context)
        {
            if (!HttpMethods.IsPost(context.Request.Method)) return false;
            var provider = context.RequestServices.GetRequiredService<Saml2pConfigurationProvider>();
            var idps = provider.GetIdentityProviderConfigurations();
            foreach(var idp in idps)
            {
                // TODO: maybe change SSO endpoint to PathString or string
                var sso = new PathString(idp.SsoEndpoint.OriginalString);
                if (sso == context.Request.Path) return true;
            }
            return false;
        }

        private static bool CanInitiateSso(HttpContext context)
        {
            if (!HttpMethods.IsGet(context.Request.Method)) return false;
            var provider = context.RequestServices.GetRequiredService<Saml2pConfigurationProvider>();
            var idps = provider.GetIdentityProviderConfigurations();
            foreach (var idp in idps)
            {
                // TODO: maybe change SSO endpoint to PathString or string
                var sso = new PathString(idp.SsoEndpoint.OriginalString).Add("/initiate");
                if (sso == context.Request.Path) return true;
            }
            return false;
        }

        private static bool CanCompleteSso(HttpContext context)
        {
            if (!HttpMethods.IsGet(context.Request.Method)) return false;
            var provider = context.RequestServices.GetRequiredService<Saml2pConfigurationProvider>();
            var idps = provider.GetIdentityProviderConfigurations();
            foreach (var idp in idps)
            {
                // TODO: maybe change SSO endpoint to PathString or string
                var sso = new PathString(idp.SsoEndpoint.OriginalString).Add("/complete");
                if (sso == context.Request.Path) return true;
            }
            return false;
        }
    }
}
