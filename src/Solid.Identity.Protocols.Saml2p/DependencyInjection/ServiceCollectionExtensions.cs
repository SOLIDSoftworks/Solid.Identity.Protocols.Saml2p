using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens.Saml2;
using Solid.Identity.Protocols.Saml2p.Abstractions;
using Solid.Identity.Protocols.Saml2p.Cache;
using Solid.Identity.Protocols.Saml2p.Options;
using Solid.Identity.Protocols.Saml2p.Factories;
using Solid.Identity.Protocols.Saml2p.Models;
using Solid.Identity.Protocols.Saml2p.Models.Context;
using Solid.Identity.Protocols.Saml2p.Providers;
using Solid.Identity.Protocols.Saml2p.Serialization;
using Solid.Identity.Protocols.Saml2p.Services;
using System.Collections.Generic;
using Solid.Identity.Protocols.Saml2p.Middleware.Sp;
using Microsoft.IdentityModel.Tokens;
using Solid.IdentityModel.Tokens;

namespace Microsoft.Extensions.DependencyInjection
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class Solid_Identity_Protocols_Saml2p_ServiceCollectionExtensions
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <summary>
        /// Adds Saml2p to <paramref name="services"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> instance to add Saml2p to.</param>
        /// <param name="configure">An action to configure <see cref="Saml2pOptions"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance so that additional calls can be chained.</returns>
        public static IServiceCollection AddSaml2p(this IServiceCollection services, Action<Saml2pOptions> configure)
        {
            services.AddDistributedMemoryCache();
            services.AddHttpContextAccessor();
            services.AddSaml2EncryptedSecurityTokenHandler();
            services.AddCustomCryptoProvider(options => options.AddFullSupport());
            services.AddMvcCore().AddRazorViewEngine();

            services.TryAddEnumerable(ServiceDescriptor.Singleton<IConfigureOptions<Saml2pOptions>, EnsureCryptoProviderFactory>());
            services.Configure(configure);
            services.PostConfigure<Saml2pOptions>(PostConfigureSaml2pOptions);

            services.TryAddTransient<StartSsoEndpointMiddleware>();
            services.TryAddTransient<FinishSsoEndpointMiddleware>();

            services.TryAddTransient<SamlResponseFactory>();
            services.TryAddTransient<ISecurityTokenDescriptorFactory, SecurityTokenDescriptorFactory>();
            services.TryAddTransient<TokenValidationParametersFactory>();
            services.TryAddTransient<AuthnRequestFactory>();
            services.TryAddTransient<Saml2pEncodingService>();
            services.TryAddTransient<IXmlWriterFactory, XmlWriterFactory>();
            services.TryAddTransient<IXmlReaderFactory, XmlReaderFactory>();
            services.TryAddTransient<Saml2pSerializer>();
            services.TryAddTransient<Saml2pPartnerProvider>();
            services.TryAddTransient<Saml2pCache>();
            services.TryAddSingleton<PathPrefixProvider>();
            services.TryAddSingleton<RazorPageRenderingService>();
            services.AddSaml2pServiceProviderClaimStore<PassthroughClaimsProvider>();

            return services;
        }

        /// <summary>
        /// Adds an implementation of <see cref="IServiceProviderClaimsProvider"/>.
        /// <para>This service is registered in <see cref="ServiceLifetime.Transient"/> scope.</para>
        /// </summary>
        /// <typeparam name="TClaimStore">The type to register.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> instance to add service to.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance so that additional calls can be chained.</returns>
        public static IServiceCollection AddSaml2pServiceProviderClaimStore<TClaimStore>(this IServiceCollection services)
            where TClaimStore : class, IServiceProviderClaimsProvider
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient<IServiceProviderClaimsProvider, TClaimStore>());
            return services;
        }


        /// <summary>
        /// Adds an implementation of <see cref="ISaml2pPartnerStore"/>.
        /// <para>This service is registered in <see cref="ServiceLifetime.Transient"/> scope.</para>
        /// </summary>
        /// <typeparam name="TStore">The type to register.</typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> instance to add service to.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance so that additional calls can be chained.</returns>
        public static IServiceCollection AddSaml2pPartnerStore<TStore>(this IServiceCollection services)
            where TStore : class, ISaml2pPartnerStore
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient<ISaml2pPartnerStore, TStore>());
            return services;
        }

        static void PostConfigureSaml2pOptions(Saml2pOptions options)
        {
            options.SupportedBindings = ((List<BindingType>)options.SupportedBindings).AsReadOnly();

            foreach (var sp in options.ServiceProviders.Values.OfType<Saml2pServiceProvider>())
            {
                sp.SupportedBindings = ((List<BindingType>)sp.SupportedBindings).AsReadOnly();
                sp.RequiredClaims = ((List<string>)sp.RequiredClaims).AsReadOnly();
                sp.OptionalClaims = ((List<string>)sp.OptionalClaims).AsReadOnly();
            }
            foreach (var idp in options.IdentityProviders.Values.OfType<Saml2pIdentityProvider>())
            {
                idp.SupportedBindings = ((List<BindingType>)idp.SupportedBindings).AsReadOnly();
                idp.AssertionSigningKeys = ((List<SecurityKey>)idp.AssertionSigningKeys).AsReadOnly();
            }
        }
    }
}
