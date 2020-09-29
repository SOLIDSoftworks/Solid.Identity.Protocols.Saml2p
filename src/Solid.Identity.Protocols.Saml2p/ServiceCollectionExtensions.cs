using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens.Saml2;
using Solid.Identity.Protocols.Saml2p.Abstractions.Factories;
using Solid.Identity.Protocols.Saml2p.Abstractions.Services;
using Solid.Identity.Protocols.Saml2p.Cache;
using Solid.Identity.Protocols.Saml2p.Options;
using Solid.Identity.Protocols.Saml2p.Factories;
using Solid.Identity.Protocols.Saml2p.Models;
using Solid.Identity.Protocols.Saml2p.Models.Context;
using Solid.Identity.Protocols.Saml2p.Providers;
using Solid.Identity.Protocols.Saml2p.Serialization;
using Solid.Identity.Protocols.Saml2p.Services;
using Solid.Identity.Tokens.Saml2;

namespace Microsoft.Extensions.DependencyInjection
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public static class Solid_Identity_Protocols_Saml2p_ServiceCollectionExtensions
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        /// <summary>
        /// Adds services for a local SAML2P service provider.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> instance to add the services to.</param>
        /// <param name="id">
        /// The name of the service provider configuration.
        /// <para><paramref name="id"/> is used to pre-configure <see cref="Saml2pProvider.Id"/> and <see cref="Saml2pProvider.Name"/>.</para>
        /// </param>
        /// <param name="configure">An action to configure <see cref="Saml2pServiceProviderOptions"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance so that additional calls can be chained.</returns>
        public static IServiceCollection AddSaml2pServiceProvider(this IServiceCollection services, string id, Action<Saml2pServiceProviderOptions> configure)
        {
            services.TryAddTransient<TokenValidationParametersFactory>();
            services.TryAddTransient<AuthnRequestFactory>();
            services.TryAddTransient<ISaml2pServiceProviderService, Saml2pServiceProviderService>();

            return services
                .AddSaml2p()
                .Configure<Saml2pServiceProviderOptions>(id, sp =>
                {
                    sp.Id = id;
                    sp.Name = id;
                })
                .Configure(id, configure)
                .PostConfigure<Saml2pServiceProviderOptions>(id, PostConfigureLocalSaml2pServiceProvider)
                .AddTransient(p => p.GetRequiredService<IOptionsSnapshot<Saml2pServiceProviderOptions>>().Get(id))
            ;
        }

        /// <summary>
        /// Adds services for a local SAML2P identity provider.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> instance to add the services to.</param>
        /// <param name="id">
        /// The name of the identity provider configuration.
        /// <para><paramref name="id"/> is used to pre-configure <see cref="Saml2pProvider.Id"/> and <see cref="Saml2pProvider.Name"/>.</para>
        /// </param>
        /// <param name="configure">An action to configure <see cref="Saml2pIdentityProviderOptions"/>.</param>
        /// <returns>The <see cref="IServiceCollection"/> instance so that additional calls can be chained.</returns>
        public static IServiceCollection AddSaml2pIdentityProvider(this IServiceCollection services, string id, Action<Saml2pIdentityProviderOptions> configure)
        {
            services.TryAddTransient<ISaml2pIdentityProviderService, Saml2pIdentityProviderService>();
            services.TryAddTransient<SamlResponseFactory>();
            services.TryAddTransient<ISecurityTokenDescriptorFactory, SecurityTokenDescriptorFactory>();
            //services.TryAddScoped<IdentityProviderContext>();

            return services
                .AddSaml2p()
                .Configure<Saml2pIdentityProviderOptions>(id, idp =>
                {
                    idp.Id = id;
                    idp.Name = id;
                })
                .Configure(id, configure)
                .PostConfigure<Saml2pIdentityProviderOptions>(id, PostConfigureLocalSaml2IdentityProvider)
                .AddTransient(p => p.GetRequiredService<IOptionsSnapshot<Saml2pIdentityProviderOptions>>().Get(id))
            ;
        }

        static IServiceCollection AddSaml2p(this IServiceCollection services)
        {
            services.AddDistributedMemoryCache();
            services.AddHttpContextAccessor();
            services.TryAddTransient<SolidSaml2Serializer>();
            services.TryAddTransient<Saml2SecurityTokenHandler, SolidSaml2SecurityTokenHandler>();
            services.TryAddTransient<IXmlWriterFactory, XmlWriterFactory>();
            services.TryAddTransient<IXmlReaderFactory, XmlReaderFactory>();
            services.TryAddTransient<Saml2pSerializer>();
            services.TryAddTransient<Saml2pPartnerProvider>();
            services.TryAddTransient<Saml2pCache>();
            services.TryAddTransient<Saml2pOptionsProvider>();
            services.TryAddScoped<IRazorPageRenderingService, RazorPageRenderingService>();
            services.AddMvcCore().AddRazorViewEngine();

            return services;
        }

        static void PostConfigureLocalSaml2pServiceProvider(Saml2pServiceProviderOptions sp)
        {
            sp.IdentityProviders = new ReadOnlyCollection<PartnerSaml2pIdentityProvider>(sp.IdentityProviders.ToArray());
            foreach (var idp in sp.IdentityProviders)
            {
                idp.ServiceProvider = sp;
                if (idp.Id == null)
                    throw new ArgumentNullException(nameof(idp.Id), $"Partner IDP '{idp.Name}' missing Id.");
            }
        }

        static void PostConfigureLocalSaml2IdentityProvider(Saml2pIdentityProviderOptions idp)
        {
            idp.ServiceProviders = new ReadOnlyCollection<PartnerSaml2pServiceProvider>(idp.ServiceProviders.ToArray());
            foreach (var sp in idp.ServiceProviders)
            {
                if (sp.Id == null)
                    throw new ArgumentNullException(nameof(sp.Id), $"Partner SP '{sp.Name}' missing Id.");
                sp.IdentityProvider = idp;
            }
        }
    }
}
