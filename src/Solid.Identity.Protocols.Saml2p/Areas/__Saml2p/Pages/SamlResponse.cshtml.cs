using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Solid.Identity.Protocols.Saml2p.Areas.__Saml2p.Pages
{
    public class SamlResponseModel : PageModel
    {
        public string SamlResponse { get; set; }
        public string RelayState { get; set; }
        public Uri Recipient { get; set; }
    }
}