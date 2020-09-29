using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using Microsoft.Extensions.DependencyInjection;
using Solid.Identity.Protocols.Saml2p.Serialization;
using System.IO;
using Solid.Identity.Protocols.Saml2p;
using Microsoft.IdentityModel.Tokens.Saml2;
using Microsoft.IdentityModel.Tokens;
using Solid.Identity.Protocols.Saml2p.Providers;
using System.Security.Claims;

namespace Solid.Identity.Protocols.Saml2p.Authentication
{
    public class Saml2pAuthenticationHandler : RemoteAuthenticationHandler<Saml2pAuthenticationOptions>
    {
        private Saml2pPartnerProvider _provider;

        public Saml2pAuthenticationHandler(Saml2pPartnerProvider provider, IOptionsMonitor<Saml2pAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) 
            : base(options, logger, encoder, clock)
        {
            _provider = provider;
        }

        protected override Task InitializeHandlerAsync()
        {
            var idp = _provider.GetPartnerIdentityProvider(Options.IdentityProviderId);
            Options.CallbackPath = idp.ServiceProvider.AssertionConsumerServiceEndpoint;
            return base.InitializeHandlerAsync();
        }

        protected override async Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
        {
            try
            {
                var subject = await Context.FinishSsoAsync();
                var ticket = new AuthenticationTicket(subject, Scheme.Name);
                return HandleRequestResult.Success(ticket);
            }
            catch(Exception ex)
            {
                return HandleRequestResult.Fail(ex);
            }
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties) => Context.StartSsoAsync(Options.IdentityProviderId);
    }
}
