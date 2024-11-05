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
using Solid.Identity.Protocols.Saml2p.Options;
using Solid.Identity.Protocols.Saml2p.Exceptions;
// ReSharper disable InconsistentNaming

namespace Solid.Identity.Protocols.Saml2p.Authentication
{
    internal class Saml2pAuthenticationHandler : RemoteAuthenticationHandler<Saml2pAuthenticationOptions>, IDisposable
    {
        private Saml2pOptions _saml2p;
        private readonly IDisposable _optionsChangeToken;

#if NET6_0
        public Saml2pAuthenticationHandler(IOptionsMonitor<Saml2pOptions> saml2pOptionsMonitor, IOptionsMonitor<Saml2pAuthenticationOptions> monitor, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) 
            : base(monitor, logger, encoder, clock)
        {
            _saml2p = saml2pOptionsMonitor.CurrentValue;
            _optionsChangeToken = saml2pOptionsMonitor.OnChange((options, _) => _saml2p = options);
        }
#else
        public Saml2pAuthenticationHandler(IOptionsMonitor<Saml2pOptions> saml2pOptionsMonitor, IOptionsMonitor<Saml2pAuthenticationOptions> monitor, ILoggerFactory logger, UrlEncoder encoder) 
            : base(monitor, logger, encoder)
        {
            _saml2p = saml2pOptionsMonitor.CurrentValue;
            _optionsChangeToken = saml2pOptionsMonitor.OnChange((options, _) => _saml2p = options);
        }
#endif
        
        

        protected override Task InitializeHandlerAsync()
        {
            Options.CallbackPath = _saml2p.FinishPath;
            return base.InitializeHandlerAsync();
        }

        protected override async Task<HandleRequestResult> HandleRemoteAuthenticateAsync()
        {
            try
            {
                var result = await Context.FinishSsoAsync();
                if (!result.IsSuccessful)
                    throw new SamlResponseException(result.PartnerId, result.Status, result.SubStatus);

                var ticket = new AuthenticationTicket(result.Subject, result.Properties, Scheme.Name);
                return HandleRequestResult.Success(ticket);
            }
            catch(Exception ex)
            {
                return HandleRequestResult.Fail(ex);
            }
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties) => Context.StartSsoAsync(Options.IdentityProviderId);

        public void Dispose() => _optionsChangeToken?.Dispose();
    }
}
