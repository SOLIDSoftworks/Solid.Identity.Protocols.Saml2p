using Solid.Identity.Protocols.Saml2p.Providers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Solid.Identity.Protocols.Saml2p.Abstractions.Services;
using Solid.Identity.Protocols.Saml2p.Serialization;
using System.Security.Claims;
using Solid.Identity.Protocols.Saml2p.Options;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using System.Linq;
using Microsoft.Extensions.Primitives;

namespace Microsoft.AspNetCore.Http
{
    public static class Solid_Identity_Protocols_Saml2p_HttpContextExtensions
    {
        public static Task StartSsoAsync(this HttpContext context, string partnerId)
        {
            var spService = context.RequestServices.GetRequiredService<ISaml2pServiceProviderService>();
            return spService.StartSsoAsync(partnerId);
        }

        public static Task<ClaimsPrincipal> FinishSsoAsync(this HttpContext context)
        {
            var spService = context.RequestServices.GetRequiredService<ISaml2pServiceProviderService>();
            return spService.FinishSsoAsync();
        }
    }
}
