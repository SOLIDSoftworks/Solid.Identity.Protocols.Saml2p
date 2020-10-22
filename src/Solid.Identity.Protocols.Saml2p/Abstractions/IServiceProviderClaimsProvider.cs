using Solid.Identity.Protocols.Saml2p.Models;
using Solid.Identity.Protocols.Saml2p.Options;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.Saml2p.Abstractions
{
    /// <summary>
    /// An interface describing a <see cref="Claim"/>s provider for <see cref="ISaml2pServiceProvider"/>s.
    /// </summary>
    public interface IServiceProviderClaimsProvider
    {
        /// <summary>
        /// A method that checks whether <see cref="Claim"/>s can be generated for a given <paramref name="partner"/>.
        /// </summary>
        /// <param name="partner">The <see cref="ISaml2pServiceProvider"/> to generate <see cref="Claim"/>s for.</param>
        /// <returns>A <see cref="ValueTask{TResult}"/> of type <see cref="bool"/>.</returns>
        ValueTask<bool> CanGenerateClaimsAsync(ISaml2pServiceProvider partner);

        /// <summary>
        /// The <see cref="Claim"/> types offered by this <see cref="IServiceProviderClaimsProvider"/>.
        /// </summary>
        IEnumerable<ClaimDescriptor> ClaimTypesOffered { get; }

        /// <summary>
        /// Generates <see cref="Claim"/>s for a given <paramref name="partner"/>.
        /// </summary>
        /// <param name="identity">The current identity used to generate <see cref="Claim"/>s.</param>
        /// <param name="partner">The <see cref="ISaml2pServiceProvider"/> to generate <see cref="Claim"/>s for.</param>
        /// <param name="issuer"></param>
        /// <returns>A <see cref="ValueTask{TResult}"/> of type <see cref="IEnumerable{T}"/> of <see cref="Claim"/>.</returns>
        ValueTask<IEnumerable<Claim>> GenerateClaimsAsync(ClaimsIdentity identity, ISaml2pServiceProvider partner, string issuer);
    }
}
