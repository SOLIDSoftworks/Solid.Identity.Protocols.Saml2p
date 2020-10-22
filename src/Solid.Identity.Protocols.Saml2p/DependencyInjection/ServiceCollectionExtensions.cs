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
using Solid.Identity.Tokens.Saml2;
using System.Collections.Generic;
using Solid.Identity.Protocols.Saml2p.Middleware.Sp;
using Microsoft.IdentityModel.Tokens;

namespace Microsoft.Extensions.DependencyInjection
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class Solid_Identity_Protocols_Saml2p_ServiceCollectionExtensions
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        ///// <summary>
        ///// Adds services for a local SAML2P service provider.
        ///// </summary>
        ///// <param name="services">The <see cref="IServiceCollection"/> instance to add the services to.</param>
        ///// <param name="id">
        ///// The name of the service provider configuration.
        ///// <para><paramref name="id"/> is used to pre-configure <see cref="Saml2pProvider.Id"/> and <see cref="Saml2pProvider.Name"/>.</para>
        ///// </param>
        ///// <param name="configure">An action to configure <see cref="Saml2pServiceProviderOptions"/>.</param>
        ///// <returns>The <see cref="IServiceCollection"/> instance so that additional calls can be chained.</returns>
        //public static IServiceCollection AddSaml2pServiceProvider(this IServiceCollection services, string id, Action<Saml2pServiceProviderOptions> configure)
        //{
        //    services.TryAddTransient<TokenValidationParametersFactory>();
        //    services.TryAddTransient<AuthnRequestFactory>();
        //    services.TryAddTransient<ISaml2pServiceProviderService, Saml2pServiceProviderService>();

        //    return services
        //        .AddRequiredServices()
        //        .Configure<Saml2pServiceProviderOptions>(id, sp =>
        //        {
        //            sp.Id = id;
        //            sp.Name = id;
        //        })
        //        .Configure(id, configure)
        //        .PostConfigure<Saml2pServiceProviderOptions>(id, PostConfigureLocalSaml2pServiceProvider)
        //        .AddTransient(p => p.GetRequiredService<IOptionsSnapshot<Saml2pServiceProviderOptions>>().Get(id))
        //    ;
        //}

        ///// <summary>
        ///// Adds services for a local SAML2P identity provider.
        ///// </summary>
        ///// <param name="services">The <see cref="IServiceCollection"/> instance to add the services to.</param>
        ///// <param name="id">
        ///// The name of the identity provider configuration.
        ///// <para><paramref name="id"/> is used to pre-configure <see cref="Saml2pProvider.Id"/> and <see cref="Saml2pProvider.Name"/>.</para>
        ///// </param>
        ///// <param name="configure">An action to configure <see cref="Saml2pIdentityProviderOptions"/>.</param>
        ///// <returns>The <see cref="IServiceCollection"/> instance so that additional calls can be chained.</returns>
        //public static IServiceCollection AddSaml2pIdentityProvider(this IServiceCollection services, string id, Action<Saml2pIdentityProviderOptions> configure)
        //{
        //    //services.TryAddTransient<ISaml2pIdentityProviderService, Saml2pIdentityProviderService>();
        //    services.TryAddTransient<SamlResponseFactory>();
        //    services.TryAddTransient<ISecurityTokenDescriptorFactory, SecurityTokenDescriptorFactory>();
        //    //services.TryAddScoped<IdentityProviderContext>();

        //    return services
        //        .AddRequiredServices()
        //        .Configure<Saml2pIdentityProviderOptions>(id, idp =>
        //        {
        //            idp.Id = id;
        //            idp.Name = id;
        //        })
        //        .Configure(id, configure)
        //        .PostConfigure<Saml2pIdentityProviderOptions>(id, PostConfigureLocalSaml2IdentityProvider)
        //        .AddTransient(p => p.GetRequiredService<IOptionsMonitor<Saml2pIdentityProviderOptions>>().Get(id))
        //    ;
        //}

        public static IServiceCollection AddSaml2p(this IServiceCollection services, Action<Saml2pOptions> configure)
        {
            services.AddDistributedMemoryCache();
            services.AddHttpContextAccessor();
            services.AddMvcCore().AddRazorViewEngine();

            services.Configure(configure);
            services.PostConfigure<Saml2pOptions>(PostConfigureSaml2pOptions);

            services.TryAddTransient<StartSsoEndpointMiddleware>();
            services.TryAddTransient<FinishSsoEndpointMiddleware>();

            services.TryAddTransient<SamlResponseFactory>();
            services.TryAddTransient<ISecurityTokenDescriptorFactory, SecurityTokenDescriptorFactory>();
            services.TryAddTransient<TokenValidationParametersFactory>();
            services.TryAddTransient<AuthnRequestFactory>();
            services.TryAddTransient<Saml2pEncodingService>();
            services.TryAddTransient<Saml2Serializer, SolidSaml2Serializer>();
            services.TryAddTransient<Saml2SecurityTokenHandler, SolidSaml2SecurityTokenHandler>();
            services.TryAddTransient<IXmlWriterFactory, XmlWriterFactory>();
            services.TryAddTransient<IXmlReaderFactory, XmlReaderFactory>();
            services.TryAddTransient<Saml2pSerializer>();
            services.TryAddTransient<Saml2pPartnerProvider>();
            services.TryAddTransient<Saml2pCache>();
            services.TryAddSingleton<PathPrefixProvider>();
            services.TryAddSingleton<RazorPageRenderingService>();
            services.AddSaml2pServiceProviderClaimStore<RequiredClaimsProvider>();
            services.AddSaml2pServiceProviderClaimStore<PassthroughClaimsProvider>();

            return services;
        }

        public static IServiceCollection AddSaml2pServiceProviderClaimStore<TClaimStore>(this IServiceCollection services)
            where TClaimStore : class, IServiceProviderClaimsProvider
        {
            services.TryAddEnumerable(ServiceDescriptor.Transient<IServiceProviderClaimsProvider, TClaimStore>());
            return services;
        }

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
