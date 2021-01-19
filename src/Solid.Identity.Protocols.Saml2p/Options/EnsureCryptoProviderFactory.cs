using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace Solid.Identity.Protocols.Saml2p.Options
{
    internal class EnsureCryptoProviderFactory : IConfigureOptions<Saml2pOptions>
    {
        private CryptoProviderFactory _cryptoProviderFactory;

        public EnsureCryptoProviderFactory(CryptoProviderFactory cryptoProviderFactory)
        {
            _cryptoProviderFactory = cryptoProviderFactory;
        }

        public void Configure(Saml2pOptions _)
        {
            if (CryptoProviderFactory.Default.CustomCryptoProvider != null) return;
            CryptoProviderFactory.Default = _cryptoProviderFactory; // this should never be hit
        }
    }
}
