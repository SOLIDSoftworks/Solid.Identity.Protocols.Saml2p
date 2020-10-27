using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Solid.Identity.Protocols.Saml2p.Areas.__Saml2p.Pages
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class AuthnRequestModel : PageModel
    {
        public Uri Destination { get; set; }
        public string SamlRequest { get; set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}