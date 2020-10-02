using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Solid.Identity.Protocols.Saml2p.Options;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Solid.Identity.Protocols.Saml2p.Providers;
using System.Linq;
using System.Threading.Tasks;
using Solid.Identity.Protocols.Saml2p.Middleware;

namespace Microsoft.AspNetCore.Builder
{
    public static class Solid_Identity_Protocols_Saml2p_ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSaml2pIdentityProvider(this IApplicationBuilder builder, PathString path, string idpId)
        {
            return builder
                .MapPost(path, b => b.UseAcceptSsoEndpoint(path, idpId))
                .MapGet(path.Add("/initiate"), b => b.UseInitiateSsoEndpoint(path, idpId))
                .MapGet(path.Add("/complete"), b => b.UseCompleteSsoEndpoint(path, idpId))
            ;
        }

        internal static IApplicationBuilder UseAcceptSsoEndpoint(this IApplicationBuilder builder, PathString path, string idpId) => builder.UsePathBase(path).UseMiddleware<AcceptSsoEndpointMiddleware>(idpId);
        internal static IApplicationBuilder UseInitiateSsoEndpoint(this IApplicationBuilder builder, PathString path, string idpId) => builder.UsePathBase(path).UseMiddleware<InitiateSsoEndpointMiddleware>(idpId);
        internal static IApplicationBuilder UseCompleteSsoEndpoint(this IApplicationBuilder builder, PathString path, string idpId) => builder.UsePathBase(path).UseMiddleware<CompleteSsoEndpointMiddleware>(idpId);

        private static IApplicationBuilder MapGet(this IApplicationBuilder builder, PathString path, Action<IApplicationBuilder> configure)
            => builder.MapMethod(path, HttpMethods.Get, configure);

        private static IApplicationBuilder MapPost(this IApplicationBuilder builder, PathString path, Action<IApplicationBuilder> configure)
            => builder.MapMethod(path, HttpMethods.Post, configure);

        private static IApplicationBuilder MapMethod(this IApplicationBuilder builder, PathString path, string method, Action<IApplicationBuilder> configure)
            => builder
                .MapWhen(
                    context => context.Request.Path.Equals(path) && context.Request.Method.Equals(method),
                    b => configure(b)
                )
                .MapWhen(
                    context => context.Request.Path.Equals(path) && !context.Request.Method.Equals(method),
                    b => b.Use((context, _) =>
                    {
                        context.Response.StatusCode = 405;
                        return Task.CompletedTask;
                    })
                )
            ;
    }
}
