using Solid.Identity.Protocols.Saml2p.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.Saml2p.Abstractions.Services
{
    public interface ISaml2pIdentityProviderService
    {
        //Task AcceptSsoAsync();
        Task AcceptSsoAsAsync(Saml2pIdentityProviderOptions idp);
        //Task InitiateSsoAsync();
        Task InitiateSsoAsAsync(Saml2pIdentityProviderOptions idp);
        //Task CompleteSsoAsync();
        Task CompleteSsoAsAsync(Saml2pIdentityProviderOptions idp);
    }
}
