using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens.Saml;
using Solid.Identity.Protocols.Saml2p;

namespace AspNetCore.IdpSample.Pages
{
    public class FauxKerberosModel : PageModel
    {
        public async Task OnGet(string returnUrl)
        {
            var userName = "KerberosUser";
            var claims = new List<Claim>();
            claims.Add(new Claim(ClaimTypes.NameIdentifier, userName));
            claims.Add(new Claim(ClaimTypes.AuthenticationInstant, DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")));
            claims.Add(new Claim(ClaimTypes.AuthenticationMethod, Saml2pConstants.Classes.KerberosString));
            claims.Add(new Claim(ClaimTypes.Name, userName));

            var identity = new ClaimsIdentity(claims, "Kerberos", ClaimTypes.NameIdentifier, ClaimTypes.Role);
            var subject = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync(subject);
            HttpContext.Response.Redirect(returnUrl);
        }
    }
}