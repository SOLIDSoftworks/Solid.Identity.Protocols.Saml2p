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
    public static class Solid_Identity_Protocols_Saml2p_ServiceCollectionExtensions
    {
        public static IServiceCollection AddSaml2pServiceProvider(this IServiceCollection services, string id, Action<Saml2pServiceProviderOptions> configure)
        {
            services.TryAddTransient<TokenValidationParametersFactory>();
            services.TryAddTransient<AuthnRequestFactory>();
            services.TryAddTransient<ISaml2pServiceProviderService, Saml2pServiceProviderService>();

            return services
                .AddSaml2p()
                .Configure<Saml2pServiceProviderOptions>(id, sp => sp.Id = id)
                .Configure(id, configure)
                .PostConfigure<Saml2pServiceProviderOptions>(id, PostConfigureLocalSaml2pServiceProvider)
                .AddTransient(p => p.GetRequiredService<IOptionsSnapshot<Saml2pServiceProviderOptions>>().Get(id))
            ;
        }

        public static IServiceCollection AddSaml2pIdentityProvider(this IServiceCollection services, string id, Action<Saml2pIdentityProviderOptions> configure)
        {
            services.TryAddTransient<ISaml2pIdentityProviderService, Saml2pIdentityProviderService>();
            services.TryAddTransient<SamlResponseFactory>();
            services.TryAddTransient<ISecurityTokenDescriptorFactory, SecurityTokenDescriptorFactory>();
            //services.TryAddScoped<IdentityProviderContext>();

            return services
                .AddSaml2p()
                .Configure<Saml2pIdentityProviderOptions>(id, idp => idp.Id = id)
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
            services.TryAddTransient<Saml2pConfigurationProvider>();
            services.TryAddScoped<IRazorPageRenderingService, RazorPageRenderingService>();
            services.AddMvcCore().AddRazorViewEngine();

            return services;
        }

        static void PostConfigureLocalSaml2pServiceProvider(Saml2pServiceProviderOptions sp)
        {
            sp.IdentityProviders = new ReadOnlyCollection<PartnerSaml2pIdentityProvider>(sp.IdentityProviders.ToArray());
            foreach (var idp in sp.IdentityProviders)
                idp.ServiceProvider = sp;
        }

        static void PostConfigureLocalSaml2IdentityProvider(Saml2pIdentityProviderOptions idp)
        {
            idp.ServiceProviders = new ReadOnlyCollection<PartnerSaml2pServiceProvider>(idp.ServiceProviders.ToArray());
            foreach (var sp in idp.ServiceProviders)
                sp.IdentityProvider = idp;
        }
    }
}
