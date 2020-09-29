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
        public static IApplicationBuilder UseAllSaml2pIdentityProviders(this IApplicationBuilder builder)
        {
            var provider = builder.ApplicationServices.GetRequiredService<Saml2pOptionsProvider>();
            foreach (var idp in provider.GetAllIdentityProviderOptions())
                builder.UseSaml2pIdentityProvider(idp);
            return builder;
        }

        public static IApplicationBuilder UseAllSaml2pIdentityProviders(this IApplicationBuilder builder, PathString pathPrefix)
        {
            return builder
                .Map(pathPrefix, b => b.UseAllSaml2pIdentityProviders())
            ;
        }

        public static IApplicationBuilder UseSaml2pIdentityProvider(this IApplicationBuilder builder, PathString pathPrefix, string idpId)
        {
            var prefixes = builder.ApplicationServices.GetRequiredService<PathPrefixProvider>();
            prefixes.SetPrefix(idpId, pathPrefix);

            return builder
                .Map(pathPrefix, b => b.UseSaml2pIdentityProvider(idpId))
            ;
        }

        public static IApplicationBuilder UseSaml2pIdentityProvider(this IApplicationBuilder builder, string idpId)
        {
            var provider = builder.ApplicationServices.GetRequiredService<Saml2pOptionsProvider>();
            var idp = provider.GetIdentityProviderOptions(idpId);
            return builder.UseSaml2pIdentityProvider(idp);
        }

        private static IApplicationBuilder UseSaml2pIdentityProvider(this IApplicationBuilder builder, Saml2pIdentityProviderOptions idp)
        {
            return builder
                .MapWhen(context => context.CanAcceptSsoAs(idp), b => b.Use((context, _) => context.AcceptSsoAsAsync(idp)))
                .MapWhen(context => context.CanInitiateSsoAs(idp), b => b.Use((context, _) => context.InitiateSsoAsAsync(idp)))
                .MapWhen(context => context.CanCompleteSsoAs(idp), b => b.Use((context, _) => context.CompleteSsoAsAsync(idp)))
            ;
        }
    }
}
