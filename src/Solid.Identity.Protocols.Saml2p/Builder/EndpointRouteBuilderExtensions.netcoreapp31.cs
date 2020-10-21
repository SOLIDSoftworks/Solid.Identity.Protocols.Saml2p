#if NETCOREAPP3_1
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Solid.Identity.Protocols.Saml2p.Providers;
using Solid.Identity.Protocols.Saml2p.Options;

namespace Microsoft.AspNetCore.Builder
{
    public static class EndpointRouteBuilderExtensions
    {
        public static IEndpointRouteBuilder MapSaml2pIdentityProvider(this IEndpointRouteBuilder builder, PathString path)
        {
            //var prefixes = builder.ServiceProvider.GetRequiredService<PathPrefixProvider>();
            //prefixes.SetPrefix(idp.Id, pathPrefix);

            builder.Map(path, builder.CreateApplicationBuilder().UseAcceptSsoEndpoint(path).Build()).WithMetadata(new HttpMethodMetadata(new[] { HttpMethods.Get, HttpMethods.Post }));
            builder.Map(path.Add("/initiate"), builder.CreateApplicationBuilder().UseInitiateSsoEndpoint(path).Build()).WithMetadata(new HttpMethodMetadata(new[] { HttpMethods.Get, HttpMethods.Post })); ;
            builder.Map(path.Add("/complete"), builder.CreateApplicationBuilder().UseCompleteSsoEndpoint(path).Build()).WithMetadata(new HttpMethodMetadata(new[] { HttpMethods.Get, HttpMethods.Post })); ;
            return builder;
        }
    }
}
#endif