using Solid.Identity.Protocols.Saml2p.Providers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Solid.Identity.Protocols.Saml2p.Abstractions.Services;
using Solid.Identity.Protocols.Saml2p.Serialization;
using System.Security.Claims;
using Solid.Identity.Protocols.Saml2p.Configuration;

namespace Microsoft.AspNetCore.Http
{
    public static class Solid_Identity_Protocols_Saml2p_HttpContextExtensions
    {
        // Accept AuthnRequest (AcceptSso)
        // Accept SamlResponse (CompleteSso)
        // Send AuthnRequest (StartSso)
        // Send SamlResponse (InitiateSso)

        public static async Task AcceptSsoAsync(this HttpContext context)
        {
            var idp = context.RequestServices.GetRequiredService<ISaml2pIdentityProviderService>();
            await idp.AcceptSsoAsync();
        }

        public static async Task InitiateSsoAsync(this HttpContext context)
        {
            var idp = context.RequestServices.GetRequiredService<ISaml2pIdentityProviderService>();
            await idp.InitiateSsoAsync();
        }

        public static async Task CompleteSsoAsync(this HttpContext context)
        {
            var idp = context.RequestServices.GetRequiredService<ISaml2pIdentityProviderService>();
            await idp.CompleteSsoAsync();
        }

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
