using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.Saml2p.Abstractions.Services
{
    public interface ISsoService
    {
        Task InitiateSsoAsync(HttpContext context, string partnerid);
    }
}
