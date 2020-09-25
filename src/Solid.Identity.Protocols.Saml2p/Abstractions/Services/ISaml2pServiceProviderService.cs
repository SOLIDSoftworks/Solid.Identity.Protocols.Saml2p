using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.Saml2p.Abstractions.Services
{
    public interface ISaml2pServiceProviderService
    {
        Task StartSsoAsync(string partnerId);
        Task<ClaimsPrincipal> FinishSsoAsync();
    }
}
