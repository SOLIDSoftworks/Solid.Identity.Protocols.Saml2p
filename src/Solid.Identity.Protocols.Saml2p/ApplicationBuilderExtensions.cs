using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Solid.Identity.Protocols.Saml2p.Options;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Solid.Identity.Protocols.Saml2p.Providers;
using System.Linq;

namespace Microsoft.AspNetCore.Builder
{
    public static class Solid_Identity_Protocols_Saml2p_ApplicationBuilderExtensions
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

            //var provider = builder.ApplicationServices.GetRequiredService<Saml2pOptionsProvider>();

            return builder
                .MapWhen(CanAcceptSso, b => b.Use((context, _) => context.AcceptSsoAsync()))
                .MapWhen(CanInitiateSso, b => b.Use((context, _) => context.InitiateSsoAsync()))
                .MapWhen(CanCompleteSso, b => b.Use((context, _) => context.CompleteSsoAsync()))
            ;
        }

        private static bool CanAcceptSso(HttpContext context)
        {
            if (!HttpMethods.IsPost(context.Request.Method)) return false;
            var provider = context.RequestServices.GetRequiredService<Saml2pOptionsProvider>();
            var idps = provider.GetIdentityProviderConfigurations();

            var current = context.Request.PathBase.Add(context.Request.Path);
            return idps.Any(idp => idp.SsoEndpoint == current);
        }

        private static bool CanInitiateSso(HttpContext context)
        {
            if (!HttpMethods.IsGet(context.Request.Method)) return false;
            var provider = context.RequestServices.GetRequiredService<Saml2pOptionsProvider>();
            var idps = provider.GetIdentityProviderConfigurations();

            var current = context.Request.PathBase.Add(context.Request.Path);
            return idps.Any(idp => idp.SsoEndpoint.Add("/initiate") == current);
        }

        private static bool CanCompleteSso(HttpContext context)
        {
            if (!HttpMethods.IsGet(context.Request.Method)) return false;
            var provider = context.RequestServices.GetRequiredService<Saml2pOptionsProvider>();
            var idps = provider.GetIdentityProviderConfigurations();

            var current = context.Request.PathBase.Add(context.Request.Path);
            return idps.Any(idp => idp.SsoEndpoint.Add("/complete") == current);
        }
    }
}
