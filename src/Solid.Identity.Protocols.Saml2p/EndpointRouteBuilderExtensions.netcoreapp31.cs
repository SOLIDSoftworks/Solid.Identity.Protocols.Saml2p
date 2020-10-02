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
        public static IEndpointRouteBuilder MapSaml2pIdentityProvider(this IEndpointRouteBuilder builder, PathString path, string idpId)
        {
            //var prefixes = builder.ServiceProvider.GetRequiredService<PathPrefixProvider>();
            //prefixes.SetPrefix(idp.Id, pathPrefix);

            builder.MapPost(path, builder.CreateApplicationBuilder().UseAcceptSsoEndpoint(path, idpId).Build());
            builder.MapGet(path.Add("/initiate"), builder.CreateApplicationBuilder().UseInitiateSsoEndpoint(path, idpId).Build());
            builder.MapGet(path.Add("/complete"), builder.CreateApplicationBuilder().UseCompleteSsoEndpoint(path, idpId).Build());
            return builder;
        }
    }
}
#endif