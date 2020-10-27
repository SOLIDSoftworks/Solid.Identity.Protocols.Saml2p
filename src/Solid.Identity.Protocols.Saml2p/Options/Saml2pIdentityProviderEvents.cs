using Microsoft.IdentityModel.Tokens.Saml2;
using Solid.Identity.Protocols.Saml2p.Models.Context;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.Saml2p.Options
{
    /// <summary>
    /// Events that are run during IDP flows.
    /// </summary>
    public class Saml2pIdentityProviderEvents
    {
        /// <summary>
        /// This is run when an SP sends an <see cref="AuthnRequest"/> to your accept sso endpoint.
        /// <para>You can manually set <see cref="AcceptSsoContext.AuthenticationScheme"/> and/or replace <see cref="AcceptSsoContext.ReturnUrl"/> here.</para>
        /// </summary>
        public Func<IServiceProvider, AcceptSsoContext, ValueTask> OnAcceptSso { get; set; } = (_, __) => new ValueTask();

        /// <summary>
        /// This is run before initiate SSO by sending a <see cref="SamlResponse"/> to an SP.
        /// <para>You can manually set <see cref="AcceptSsoContext.AuthenticationScheme"/> and/or replace <see cref="InitiateSsoContext.ReturnUrl"/> here.</para>
        /// </summary>
        public Func<IServiceProvider, InitiateSsoContext, ValueTask> OnInitiateSso { get; set; } = (_, __) => new ValueTask();

        /// <summary>
        /// This is run when completing SSO by responding to an <see cref="AuthnRequest"/> from an SP with a <see cref="SamlResponse"/>.
        /// </summary>
        public Func<IServiceProvider, CompleteSsoContext, ValueTask> OnCompleteSso { get; set; } = (_, __) => new ValueTask();

        /// <summary>
        /// This is run before the <see cref="Saml2SecurityToken"/> is created.
        /// <para>You can manually populate <see cref="CreateSecurityTokenContext.SecurityToken"/> here.</para>
        /// </summary>
        public Func<IServiceProvider, CreateSecurityTokenContext, ValueTask> OnCreatingSecurityToken { get; set; } = (_, __) => new ValueTask();

        /// <summary>
        /// This is run after the <see cref="Saml2SecurityToken"/> is created.
        /// </summary>
        public Func<IServiceProvider, CreateSecurityTokenContext, ValueTask> OnCreatedSecurityToken { get; set; } = (_, __) => new ValueTask();
    }
}
