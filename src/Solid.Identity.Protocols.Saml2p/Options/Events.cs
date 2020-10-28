using Solid.Identity.Protocols.Saml2p.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.Saml2p.Options
{
    internal static class Events
    {
        public static async ValueTask InvokeAsync(Saml2pOptions options, ISaml2pIdentityProvider idp, Func<Saml2pServiceProviderEvents, ValueTask> method)
        {
            await method(options.ServiceProviderEvents);
            if (idp?.Events != null)
                await method(idp.Events);
        }

        public static async ValueTask InvokeAsync(Saml2pOptions options, ISaml2pServiceProvider sp, Func<Saml2pIdentityProviderEvents, ValueTask> method)
        {
            await method(options.IdentityProviderEvents);
            if (sp?.Events != null)
                await method(sp.Events);
        }
    }
}
