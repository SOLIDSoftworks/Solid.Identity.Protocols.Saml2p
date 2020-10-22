﻿using Microsoft.AspNetCore.Http;
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
using Solid.Identity.Protocols.Saml2p.Middleware.Idp;
using Solid.Identity.Protocols.Saml2p.Middleware.Sp;

namespace Microsoft.AspNetCore.Builder
{
    public static class Solid_Identity_Protocols_Saml2p_ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseSaml2pIdentityProvider(this IApplicationBuilder builder, PathString path)
        {
            var options = builder.ApplicationServices.GetRequiredService<IOptions<Saml2pOptions>>().Value;
            return builder
                .Map(path.Add(options.AcceptPath), b => b.UseAcceptSsoEndpoint(path))
                .Map(path.Add(options.InitiatePath), b => b.UseInitiateSsoEndpoint(path))
                .Map(path.Add(options.CompletePath), b => b.UseCompleteSsoEndpoint(path))
            ;
        }
        public static IApplicationBuilder UseSaml2pServiceProvider(this IApplicationBuilder builder, PathString path)
        {
            var options = builder.ApplicationServices.GetRequiredService<IOptions<Saml2pOptions>>().Value;
            return builder
                .Map(path.Add(options.StartPath), b => b.UseStartSsoEndpoint(path))
                .Map(path.Add(options.FinishPath), b => b.UseFinishSsoEndpoint(path))
            ;
        }

        internal static IApplicationBuilder UseStartSsoEndpoint(this IApplicationBuilder builder, PathString path)
            => builder.UsePathBase(path).UseMiddleware<StartSsoEndpointMiddleware>();

        internal static IApplicationBuilder UseFinishSsoEndpoint(this IApplicationBuilder builder, PathString path)
            => builder.UsePathBase(path).UseMiddleware<FinishSsoEndpointMiddleware>();

        internal static IApplicationBuilder UseAcceptSsoEndpoint(this IApplicationBuilder builder, PathString path)
            => builder.UsePathBase(path).UseMiddleware<AcceptSsoEndpointMiddleware>();

        internal static IApplicationBuilder UseInitiateSsoEndpoint(this IApplicationBuilder builder, PathString path) 
            => builder.UsePathBase(path).UseMiddleware<InitiateSsoEndpointMiddleware>();

        internal static IApplicationBuilder UseCompleteSsoEndpoint(this IApplicationBuilder builder, PathString path) 
            => builder.UsePathBase(path).UseMiddleware<CompleteSsoEndpointMiddleware>();

        private static IApplicationBuilder MapGet(this IApplicationBuilder builder, PathString path, Action<IApplicationBuilder> configure)
            => builder.MapMethods(path, new[] { HttpMethods.Get }, configure);

        private static IApplicationBuilder MapPost(this IApplicationBuilder builder, PathString path, Action<IApplicationBuilder> configure)
            => builder.MapMethods(path, new[] { HttpMethods.Post }, configure);

        private static IApplicationBuilder MapMethods(this IApplicationBuilder builder, PathString path, string[] methods, Action<IApplicationBuilder> configure)
            => builder
                .MapWhen(
                    context => context.Request.Path.Equals(path) && methods.Contains(context.Request.Method),
                    b => configure(b)
                )
                .MapWhen(
                    context => context.Request.Path.Equals(path) && !methods.Contains(context.Request.Method),
                    b => b.Use((context, _) =>
                    {
                        context.Response.StatusCode = 405;
                        return Task.CompletedTask;
                    })
                )
            ;
    }
}