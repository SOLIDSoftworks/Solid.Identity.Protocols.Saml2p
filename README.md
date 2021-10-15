# Solid.Identity.Protocols.Saml2p
[![build](https://github.com/SOLIDSoftworks/Solid.Identity.Protocols.Saml2p/actions/workflows/build.yml/badge.svg?branch=master)](https://github.com/SOLIDSoftworks/Solid.Identity.Protocols.Saml2p/actions/workflows/build.yml)

A simple SAML2p protocol library for aspnetcore.

## Features
- SSO
- Multiple partner IDPs
- Multiple partner SPs
- Global event handlers
- Per partner event handlers
- In-memory partner store
- Optional custom partner store
- Microsoft.AspNetCore.Authentication integration
- Encrypted tokens

### Upcoming features
- SLO
- Signed requests
- Signed responses
- Federation metadata endpoint
- Federation metadata import
- Validate incoming authentication context
- Allow accept and initiate events to manually provide a status

## Usage

### Service provider (SP)
Using this package as an SP is pretty easy with the *Microsoft.AspNetCore.Authentication* integration.
```csharp

public void ConfigureServices(IServiceCollection services)
{
    services
        .AddSaml2p(options =>
        {
            // this will be the issuer of the AuthnRequest 
            // unless overridden in the parnter IDP configuration.
            options.DefaultIssuer = "https://myhost/saml";

            // The following values should come from your partner IDP.
            // If these values differ between environments, you can implement 
            // IConfigureOptions<Saml2pOptions> and add as a singleton to the
            // service collection.
            options.AddIdentityProvider("https://identityproviderhost/saml", idp =>
            {
                idp.Name = "My partner identity provider";
                idp.BaseUrl = new Uri("https://identityproviderhost");
                idp.AcceptSsoEndpoint = "/saml/sso";
                idp.CanInitiateSso = true;
                // Multiple signing keys can be added for secondary 
                // and tertiary key purposes.
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
        .AddCookie()
        .AddSaml2p("https://identityproviderhost/saml")
    ;
}

```

#### Razor or MVC
If you are using Razor or MVC, you can simply put an ```[Authorize]``` attribute on your PageModel/Controller/Method and this will just work.

#### Manual use
If you're not using Razor og MVC, you can call the extension methods for *HttpContext* included in aspnetcore.

```csharp
var result = await context.AuthenticateAsync();
if(!result.Succeeded)
    await context.ChallengeAsync();
```

### Identity provider (IDP)
Using this package as an IDP is also pretty easy with the *Microsoft.AspNetCore.Authentication* integration.
```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddSaml2p(options =>
            {
                // this will be the issuer of the Response 
                // unless overridden in the parnter SP configuration.
                options.DefaultIssuer = "https://myhost/saml";
    
                // The following values should come from your partner SP.
                // If these values differ between environments, you can implement 
                // IConfigureOptions<Saml2pOptions> and add as a singleton to the
                // service collection.
                options.AddServiceProvider("https://serviceproviderhost/saml", sp =>
                {
                    sp.BaseUrl = new Uri("https://serviceproviderhost");
                    sp.MaxClockSkew = TimeSpan.FromMinutes(2);
                    sp.AssertionConsumerServiceEndpoint = "/finish";
                    sp.AssertionSigningKey = new X509SecurityKey(new X509Certificate2(Convert.FromBase64String(SigningCertificateBase64)));
                    sp.AssertionSigningMethod = SignatureMethod.RsaSha512;

                    sp.SupportedBindings.Clear();
                    sp.SupportedBindings.Add(BindingType.Post);
                });
            })
        ;
    
        // You can implement your authentication any way you like.
        // You can even integrate with IdentityServer4 or Duende IdentityServer.
        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(o =>
            {
                o.LoginPath = "/login";
            })
        ;
    }

    public void Configure(IApplicationBuilder app)
    {
        // other middleware/endpoints added at your discretion.
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapSaml2pIdentityProvider("/saml/sso");
        });
    }
}
```
#### Responding to RequestedAuthnContext
According to the spec, if a preferred authentication context is requested, it's chosen at the discretion of the responder. This can be done with more authentication handlers on the IDP side and 

```csharp
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
            options.DefaultIssuer = "https://myhost/saml";

            options.IdentityProviderEvents.OnAcceptSso = (services, context) =>
            {
                if (context.Request.RequestedAuthnContext?.AuthnContextClassRef == Saml2pConstants.Classes.Kerberos)
                {
                    // The SAML2p AcceptSsoEndpointMiddleware will challenge using this scheme.
                    context.AuthenticationScheme = "FauxKerberos";
                }
                return new ValueTask();
            };
            options.AddServiceProvider("https://serviceproviderhost/saml", sp =>
            {
                sp.BaseUrl = new Uri("https://serviceproviderhost");
                sp.MaxClockSkew = TimeSpan.FromMinutes(2);
                sp.AssertionConsumerServiceEndpoint = "/finish";
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

    //...
}
```
