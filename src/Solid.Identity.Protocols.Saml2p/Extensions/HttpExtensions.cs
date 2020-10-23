using Solid.Identity.Protocols.Saml2p.Providers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Solid.Identity.Protocols.Saml2p.Abstractions;
using Solid.Identity.Protocols.Saml2p.Serialization;
using System.Security.Claims;
using Solid.Identity.Protocols.Saml2p.Options;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using System.Linq;
using Microsoft.Extensions.Primitives;
using Solid.Identity.Protocols.Saml2p.Middleware.Sp;
using Solid.Identity.Protocols.Saml2p.Models.Results;

namespace Microsoft.AspNetCore.Http
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class Solid_Identity_Protocols_Saml2p_HttpExtensions
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        public static Task StartSsoAsync(this HttpContext context, string partnerId)
        {
            var middleware = context.RequestServices.GetRequiredService<StartSsoEndpointMiddleware>();

            // TODO: possibly return StartSsoResult class
            return middleware.StartSsoAsync(context, partnerId);
        }

        public static async Task<FinishSsoResult> FinishSsoAsync(this HttpContext context)
        {
            var middleware = context.RequestServices.GetRequiredService<FinishSsoEndpointMiddleware>();
            return await middleware.FinishSsoAsync(context);
        }
    }
}
