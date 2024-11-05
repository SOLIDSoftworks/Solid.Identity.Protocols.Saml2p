using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
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
using Solid.Identity.Protocols.Saml2p.Authentication;
using Solid.Identity.Protocols.Saml2p.Models.Context;
using Solid.Identity.Protocols.Saml2p.Options;
using Solid.Identity.Protocols.Saml2p.Serialization;

namespace AspNetCore.SpSample
{
    public class Startup
    {
        static Startup()
        {
            IdentityModelEventSource.ShowPII = true;
        }
        public static readonly string SigningCertificateBase64 = "MIIDPDCCAiSgAwIBAgIQNl7j8AGK7J1B4E/BX+vSLzANBgkqhkiG9w0BAQUFADAgMR4wHAYDVQQDDBV0b2tlbi5zaWduYXR1cmUudGVzdHMwHhcNMjAwOTIzMTU1NTIwWhcNMzAwOTI1MTU1NTIwWjAgMR4wHAYDVQQDDBV0b2tlbi5zaWduYXR1cmUudGVzdHMwggEiMA0GCSqGSIb3DQEBAQUAA4IBDwAwggEKAoIBAQC+zSiOz1UGpHEUgd23nZ63TmX7EnbgRBqeJjcujZdyjCJvV5u/uIbSR3LBNlbS8Rv4uqYWRxotUaTSCdif/jryzHUwPORU2lt7XbeIGEK9aNv9LcxpuEu1sUo/7Ei34uJtMdoZh6cvlzGoGMcTxapQBxrmQyE6LOHkni/sA8zI+mKHbPrRUyeiL38A54Dnc4wAWWy8euQtu9bJge0qcnT0ezp41A1z/BQ6yRKioQ9jHiOgIKnBDdAhWTFPKH4Roq4lIMt8PpIy5F2VYP5rz95obFExnSwvd+8XHaHP5rjZ7yLhhSD9yZtYzLf9nw3ea6KgAAHBbg2iFJIswb1opzJlAgMBAAGjcjBwMA4GA1UdDwEB/wQEAwIFoDAdBgNVHSUEFjAUBggrBgEFBQcDAgYIKwYBBQUHAwEwIAYDVR0RBBkwF4IVdG9rZW4uc2lnbmF0dXJlLnRlc3RzMB0GA1UdDgQWBBQa8UJxJKZCMuFEbsUqLJtj1TMJODANBgkqhkiG9w0BAQUFAAOCAQEAguPDY/RnMhipwJS+6gsthlQ1lY55KMCkxEcyAJjz5pZpgfd4oG0gfmP7S2V1bxQ8hLAdYMRyF4yfUnog7YwCwecSIG0aADaksHWbQU+k51rk4d1VetZnwmRfktzs560dmprQKL9rseYZQhFbYYXe8yyFwe3fPgOJhZkIgq7eUzQRO6kXOEwRxxYmWE3XhiiALLGUA9Yb6yyLg3sQ4Myequk+W4Fxw3n9j0jCRTjye+JlycwLM+ST4Z5lFuZVLHWZqqreUYcRvYpJ9lIq7C5b/bQnJQ873rSF6jjx17E+/YrQFpJbjSJrl8cSx3QephdUWUC2Op4n051O91tM32Lrpg==";
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
                    options.DefaultIssuer = "https://localhost:5003/saml";
                    options.StartPath = "/saml2p/start";
                    options.FinishPath = "/saml2p/finish";
                    options.AddIdentityProvider("https://localhost:5001/saml", idp =>
                    {
                        idp.BaseUrl = new Uri("https://localhost:5001");
                        idp.AcceptSsoEndpoint = "/saml/sso";
                        idp.CanInitiateSso = true;
                        idp.RequestedAuthnContextClassRef = Saml2pConstants.Classes.Kerberos;
                        idp.AssertionSigningKeys.Add(new X509SecurityKey(new X509Certificate2(Convert.FromBase64String(SigningCertificateBase64))));
                    });
                })
            ;

            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = Saml2pAuthenticationDefaults.AuthenticationScheme;
                })
                .AddCookie(o =>
                 {
                     o.Cookie.Name = "Cookie.Sp";
                 })
                .AddSaml2p("https://localhost:5001/saml")
            ;

            services.AddMvc();
        }

        private ValueTask ValidatingToken(IServiceProvider arg1, ValidateTokenContext arg2)
        {
            return new ValueTask();
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
                endpoints.MapSaml2pServiceProvider("/sso");
                endpoints.MapRazorPages();
            });
        }
    }
}
