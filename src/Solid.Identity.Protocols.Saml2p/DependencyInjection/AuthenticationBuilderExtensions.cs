using Microsoft.AspNetCore.Authentication;
using Solid.Identity.Protocols.Saml2p.Abstractions;
using Solid.Identity.Protocols.Saml2p.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class Solid_Identity_Protocols_Saml2p_AuthenticationBuilderExtensions
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <summary>
        /// Registers Saml2p authentication handler with the default scheme name.
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/> instance.</param>
        /// <param name="identityProviderId">The id of the IDP to request authentication from.</param>
        /// <returns>The <see cref="AuthenticationBuilder"/> instance so that additional calls can be chained.</returns>
        public static AuthenticationBuilder AddSaml2p(this AuthenticationBuilder builder, string identityProviderId) =>
            builder.AddSaml2p(Saml2pAuthenticationDefaults.AuthenticationScheme, identityProviderId);

        /// <summary>
        /// Registers Saml2p authentication handler.
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/> instance.</param>
        /// <param name="schemeName">The scheme name.</param>
        /// <param name="identityProviderId">The id of the IDP to request authentication from.</param>
        /// <returns>The <see cref="AuthenticationBuilder"/> instance so that additional calls can be chained.</returns>
        public static AuthenticationBuilder AddSaml2p(this AuthenticationBuilder builder, string schemeName, string identityProviderId) =>
            builder.AddSaml2p(schemeName, schemeName, identityProviderId);

        /// <summary>
        /// Registers Saml2p authentication handler.
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/> instance.</param>
        /// <param name="schemeName">The scheme name.</param>
        /// <param name="displayName">The scheme display name.</param>
        /// <param name="identityProviderId">The id of the IDP to request authentication from.</param>
        /// <returns>The <see cref="AuthenticationBuilder"/> instance so that additional calls can be chained.</returns>
        public static AuthenticationBuilder AddSaml2p(this AuthenticationBuilder builder, string schemeName, string displayName, string identityProviderId) =>
            builder.AddRemoteScheme<Saml2pAuthenticationOptions, Saml2pAuthenticationHandler>(schemeName, displayName, options =>
            {
                options.IdentityProviderId = identityProviderId;
            });

        /// <summary>
        /// Registers Saml2p authentication handler with the default scheme name.
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/> instance.</param>
        /// <param name="configure">An action to configure <see cref="ISaml2pAuthenticationOptions"/>.</param>
        /// <returns>The <see cref="AuthenticationBuilder"/> instance so that additional calls can be chained.</returns>
        public static AuthenticationBuilder AddSaml2p(this AuthenticationBuilder builder, Action<ISaml2pAuthenticationOptions> configure) =>
            builder.AddSaml2p(Saml2pAuthenticationDefaults.AuthenticationScheme, configure);

        /// <summary>
        /// Registers Saml2p authentication handler.
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/> instance.</param>
        /// <param name="schemeName">The scheme name.</param>
        /// <param name="configure">An action to configure <see cref="ISaml2pAuthenticationOptions"/>.</param>
        /// <returns>The <see cref="AuthenticationBuilder"/> instance so that additional calls can be chained.</returns>
        public static AuthenticationBuilder AddSaml2p(this AuthenticationBuilder builder, string schemeName, Action<ISaml2pAuthenticationOptions> configure) =>
            builder.AddSaml2p(schemeName, schemeName, configure);

        /// <summary>
        /// Registers Saml2p authentication handler.
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/> instance.</param>
        /// <param name="schemeName">The scheme name.</param>
        /// <param name="displayName">The scheme display name.</param>
        /// <param name="configure">An action to configure <see cref="ISaml2pAuthenticationOptions"/>.</param>
        /// <returns>The <see cref="AuthenticationBuilder"/> instance so that additional calls can be chained.</returns>
        public static AuthenticationBuilder AddSaml2p(this AuthenticationBuilder builder, string schemeName, string displayName, Action<ISaml2pAuthenticationOptions> configure) =>
            builder.AddRemoteScheme<Saml2pAuthenticationOptions, Saml2pAuthenticationHandler>(schemeName, displayName, configure);
    }
}
