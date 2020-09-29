using Microsoft.AspNetCore.Authentication;
using Solid.Identity.Protocols.Saml2p.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class Solid_Identity_Protocols_Saml2p_AuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddSaml2p(this AuthenticationBuilder builder, string identityProviderId) =>
            builder.AddSaml2p(Saml2pAuthenticationDefaults.AuthenticationScheme, identityProviderId);
        public static AuthenticationBuilder AddSaml2p(this AuthenticationBuilder builder, string schemeName, string identityProviderId) =>
            builder.AddSaml2p(schemeName, schemeName, identityProviderId);
        public static AuthenticationBuilder AddSaml2p(this AuthenticationBuilder builder, string schemeName, string displayName, string identityProviderId) =>
            builder.AddRemoteScheme<Saml2pAuthenticationOptions, Saml2pAuthenticationHandler>(schemeName, displayName, options =>
            {
                options.IdentityProviderId = identityProviderId;
                options.CallbackPath = "/sso_complete";
            });
        public static AuthenticationBuilder AddSaml2p(this AuthenticationBuilder builder, Action<ISaml2pAuthenticationOptions> configure) =>
            builder.AddSaml2p(Saml2pAuthenticationDefaults.AuthenticationScheme, configure);
        public static AuthenticationBuilder AddSaml2p(this AuthenticationBuilder builder, string schemeName, Action<ISaml2pAuthenticationOptions> configure) =>
            builder.AddSaml2p(schemeName, schemeName, configure);
        public static AuthenticationBuilder AddSaml2p(this AuthenticationBuilder builder, string schemeName, string displayName, Action<ISaml2pAuthenticationOptions> configure) =>
            builder.AddRemoteScheme<Saml2pAuthenticationOptions, Saml2pAuthenticationHandler>(schemeName, displayName, configure);
    }
}
