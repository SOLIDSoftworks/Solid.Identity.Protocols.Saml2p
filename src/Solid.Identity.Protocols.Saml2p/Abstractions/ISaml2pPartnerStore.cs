using Solid.Identity.Protocols.Saml2p.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.Saml2p.Abstractions
{
    /// <summary>
    /// An interface describing an external storage for Saml2p partners.
    /// </summary>
    public interface ISaml2pPartnerStore
    {
        /// <summary>
        /// Gets a partner identity provider.
        /// </summary>
        /// <param name="id">The id of the partner.</param>
        /// <returns>An awaitable <see cref="ValueTask{TResult}"/> of type <see cref="ISaml2pIdentityProvider"/>.</returns>
        ValueTask<ISaml2pIdentityProvider> GetIdentityProviderAsync(string id);

        /// <summary>
        /// Gets a partner service provider.
        /// </summary>
        /// <param name="id">The id of the partner.</param>
        /// <returns>An awaitable <see cref="ValueTask{TResult}"/> of type <see cref="ISaml2pServiceProvider"/>.</returns>
        ValueTask<ISaml2pServiceProvider> GetServiceProviderAsync(string id);
    }
}
