#if NETCOREAPP3_1
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Solid.Identity.Protocols.Saml2p.Providers;
using Solid.Identity.Protocols.Saml2p.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EndpointRouteBuilderExtensions
    {
        public static IEndpointRouteBuilder MapAllSaml2pIdentityProviders(this IEndpointRouteBuilder builder)
            => builder.MapAllSaml2pIdentityProviders(PathString.Empty);

        public static IEndpointRouteBuilder MapAllSaml2pIdentityProviders(this IEndpointRouteBuilder builder, PathString pathPrefix)
        {
            var provider = builder.ServiceProvider.GetRequiredService<Saml2pOptionsProvider>();
            foreach (var idp in provider.GetAllIdentityProviderOptions())
                builder.MapSaml2pIdentityProvider(pathPrefix, idp);
            return builder;
        }

        public static IEndpointRouteBuilder MapSaml2pIdentityProvider(this IEndpointRouteBuilder builder, string idpId)
            => builder.MapSaml2pIdentityProvider(PathString.Empty, idpId);

        public static IEndpointRouteBuilder MapSaml2pIdentityProvider(this IEndpointRouteBuilder builder, string idpId, PathString pathPrefix)
        {
            var provider = builder.ServiceProvider.GetRequiredService<Saml2pOptionsProvider>();
            var idp = provider.GetIdentityProviderOptions(idpId);
            return builder.MapSaml2pIdentityProvider(pathPrefix, idp);

        }
        static IEndpointRouteBuilder MapSaml2pIdentityProvider(this IEndpointRouteBuilder builder, Saml2pIdentityProviderOptions idp, PathString pathPrefix)
        {
            var prefixes = builder.ServiceProvider.GetRequiredService<PathPrefixProvider>();
            prefixes.SetPrefix(idp.Id, pathPrefix);

            builder.MapPost(pathPrefix.Add(idp.SsoEndpoint), builder.CreateApplicationBuilder().Use((context, _) => context.AcceptSsoAsAsync(idp)).Build());
            builder.MapGet(pathPrefix.Add(idp.SsoEndpoint).Add("/initiate"), builder.CreateApplicationBuilder().Use((context, _) => context.InitiateSsoAsAsync(idp)).Build());
            builder.MapGet(pathPrefix.Add(idp.SsoEndpoint).Add("/complete"), builder.CreateApplicationBuilder().Use((context, _) => context.CompleteSsoAsAsync(idp)).Build());
            return builder;
        }
    }

    //private static IApplicationBuilder UseSaml2pIdentityProvider(this IApplicationBuilder builder, Saml2pIdentityProviderOptions idp)
    //{
    //    return builder
    //        .MapWhen(context => context.CanAcceptSsoAs(idp), b => b.Use((context, _) => context.AcceptSsoAsAsync(idp)))
    //        .MapWhen(context => context.CanInitiateSsoAs(idp), b => b.Use((context, _) => context.InitiateSsoAsAsync(idp)))
    //        .MapWhen(context => context.CanCompleteSsoAs(idp), b => b.Use((context, _) => context.CompleteSsoAsAsync(idp)))
    //    ;
    //}
}
#endif