using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Solid.Identity.Protocols.Saml2p.Providers;
using Solid.Identity.Protocols.Saml2p.Options;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Microsoft.AspNetCore.Builder
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class Solid_Identity_Protocols_Saml2p_EndpointRouteBuilderExtensions
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <summary>
        /// Maps the IDP endpoints to <paramref name="path"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> to map the endpoints to.</param>
        /// <param name="path">The base path to map the endpoints to.</param>
        /// <returns>The <see cref="IEndpointRouteBuilder"/> instance so that additional calls can be chained.</returns>
        public static IEndpointRouteBuilder MapSaml2pIdentityProvider(this IEndpointRouteBuilder builder, PathString path)
        {
            var options = builder.ServiceProvider.GetRequiredService<IOptions<Saml2pOptions>>().Value;

            builder.Map(path.Add(options.AcceptPath), builder.CreateApplicationBuilder().UseAcceptSsoEndpoint(path).Build()).WithMetadata(new HttpMethodMetadata(new[] { HttpMethods.Get, HttpMethods.Post }));
            builder.Map(path.Add(options.InitiatePath), builder.CreateApplicationBuilder().UseInitiateSsoEndpoint(path).Build()).WithMetadata(new HttpMethodMetadata(new[] { HttpMethods.Get, HttpMethods.Post }));
            builder.Map(path.Add(options.CompletePath), builder.CreateApplicationBuilder().UseCompleteSsoEndpoint(path).Build()).WithMetadata(new HttpMethodMetadata(new[] { HttpMethods.Get, HttpMethods.Post }));
            return builder;
        }
        
        /// <summary>
        /// Maps the SP endpoints to <paramref name="path"/>.
        /// </summary>
        /// <param name="builder">The <see cref="IEndpointRouteBuilder"/> to map the endpoints to.</param>
        /// <param name="path">The base path to map the endpoints to.</param>
        /// <returns>The <see cref="IEndpointRouteBuilder"/> instance so that additional calls can be chained.</returns>
        public static IEndpointRouteBuilder MapSaml2pServiceProvider(this IEndpointRouteBuilder builder, PathString path)
        {
            var options = builder.ServiceProvider.GetRequiredService<IOptions<Saml2pOptions>>().Value;

            builder.Map(path.Add(options.StartPath), builder.CreateApplicationBuilder().UseStartSsoEndpoint(path).Build()).WithMetadata(new HttpMethodMetadata(new[] { HttpMethods.Get, HttpMethods.Post }));
            builder.Map(path.Add(options.FinishPath), builder.CreateApplicationBuilder().UseFinishSsoEndpoint(path).Build()).WithMetadata(new HttpMethodMetadata(new[] { HttpMethods.Get, HttpMethods.Post })); 
            return builder;
        }
    }
}