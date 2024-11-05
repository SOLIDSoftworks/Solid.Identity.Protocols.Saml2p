using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Solid.Identity.Protocols.Saml2p;
using Solid.Identity.Protocols.Saml2p.Abstractions;
using Solid.Identity.Protocols.Saml2p.Models;
using Solid.Identity.Protocols.Saml2p.Options;

namespace AspNetCore.IdpSample
{
    public class Startup
    {
        private static readonly string SigningCertificateBase64 = "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";

        static Startup()
        {
            IdentityModelEventSource.ShowPII = true;
        }
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services
                .AddSaml2p(options =>
                {
                    options.DefaultIssuer = "https://localhost:5001/saml";
                    options.IdentityProviderEvents.OnAcceptSso = (services, context) =>
                    {
                        if (context.Request.RequestedAuthnContext?.AuthnContextClassRef == Saml2pConstants.Classes.Kerberos)
                        {
                            context.AuthenticationScheme = "FauxKerberos";
                            context.AuthenticationPropertyItems.Add(ClaimTypes.AuthenticationMethod, Saml2pConstants.Classes.KerberosString);
                        }
                        return new ValueTask();
                    };
                    options.AddServiceProvider("https://localhost:5003/saml", sp =>
                    {
                        sp.BaseUrl = new Uri("https://localhost:5003");
                        sp.MaxClockSkew = TimeSpan.FromMinutes(2);
                        sp.AssertionConsumerServiceEndpoint = "/saml2p/finish";
                        sp.AssertionSigningKey = new X509SecurityKey(new X509Certificate2(Convert.FromBase64String(SigningCertificateBase64)));

                        sp.SupportedBindings.Clear();
                        sp.SupportedBindings.Add(BindingType.Post);
                    });
                })
            ;

            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(o =>
                {
                    o.LoginPath = "/login";
                    o.Cookie.Name = "Cookie.Idp";
                })
                .AddCookie("FauxKerberos", o =>
                {
                    o.LoginPath = "/fauxkerberos";
                })
            ;

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapSaml2pIdentityProvider("/saml/sso");
                endpoints.MapRazorPages();
            });
        }
    }
}
