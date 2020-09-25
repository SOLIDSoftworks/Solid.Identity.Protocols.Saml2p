using Microsoft.AspNetCore.Authentication;
using Solid.Extensions.AspNetCore.Saml2p;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class Solid_AspNetCore_Authentication_Saml2p_AuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddSaml2p(this AuthenticationBuilder builder, string partnerId) =>
            builder.AddSaml2p("Saml2p", partnerId);
        public static AuthenticationBuilder AddSaml2p(this AuthenticationBuilder builder, string schemeName, string partnerId) =>
            builder.AddSaml2p(schemeName, schemeName, partnerId);
        public static AuthenticationBuilder AddSaml2p(this AuthenticationBuilder builder, string schemeName, string displayName, string partnerId) =>
            builder.AddRemoteScheme<Saml2pAuthenticationOptions, Saml2pAuthenticationHandler>(schemeName, displayName, options =>
            {
                options.PartnerId = partnerId;
                options.CallbackPath = "/sso_complete";
            });
        public static AuthenticationBuilder AddSaml2p(this AuthenticationBuilder builder, Action<ISaml2pAuthenticationOptions> configure) =>
            builder.AddSaml2p("Saml2p", configure);
        public static AuthenticationBuilder AddSaml2p(this AuthenticationBuilder builder, string schemeName, Action<ISaml2pAuthenticationOptions> configure) =>
            builder.AddSaml2p(schemeName, schemeName, configure);
        public static AuthenticationBuilder AddSaml2p(this AuthenticationBuilder builder, string schemeName, string displayName, Action<ISaml2pAuthenticationOptions> configure) =>
            builder.AddRemoteScheme<Saml2pAuthenticationOptions, Saml2pAuthenticationHandler>(schemeName, displayName, configure);
    }
}
