using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.Saml2p.Abstractions.Services
{
    public interface ISaml2pIdentityProviderService
    {
        Task AcceptSsoAsync();
        Task InitiateSsoAsync();
        Task CompleteSsoAsync();
    }
}
