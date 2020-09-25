using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Abstractions.Configuration
{
    public interface IIdentityProviderConfiguration
    {
        string Issuer { get; }
        IEnumerable<IBinding> SsoBindings { get; }
        IEnumerable<IBinding> SloBindings { get; }
        TimeSpan DefaultClockSkewTolerence { get; }
        EncryptingCredentials DefaultEncryptingCredentials { get; }
        SigningCredentials DefaultSigningCredentials { get; }
        IEnumerable<IPartnerServiceProvider> Partners { get; }
    }
}
