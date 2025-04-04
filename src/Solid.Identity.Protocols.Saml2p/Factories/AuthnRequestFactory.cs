using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Solid.Identity.Protocols.Saml2p.Options;
using Solid.Identity.Protocols.Saml2p.Models;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Options;
using Solid.Identity.Protocols.Saml2p.Models.Context;
using System.Threading.Tasks;
using Solid.Identity.Protocols.Saml2p.Abstractions;

namespace Solid.Identity.Protocols.Saml2p.Factories
{
    /// <summary>
    /// A factory for creating <see cref="AuthnRequest"/>s.
    /// </summary>
    public class AuthnRequestFactory
    {
        private readonly Saml2pOptions _options;
#if NET6_0
        private readonly ISystemClock _systemClock;
        /// <summary>
        /// Creates an instance of <see cref="AuthnRequestFactory"/>.
        /// </summary>
        /// <param name="systemClock">An interface that abstracts the system clock.</param>
        /// <param name="options">The current <see cref="Saml2pOptions" />.</param>
        public AuthnRequestFactory(ISystemClock systemClock, IOptions<Saml2pOptions> options)
        {
            _systemClock = systemClock;
            _options = options.Value;
        }
#else
        private readonly TimeProvider _time;
        /// <summary>
        /// Creates an instance of <see cref="AuthnRequestFactory"/>.
        /// </summary>
        /// <param name="time">The provider of the current time.</param>
        /// <param name="options">The current <see cref="Saml2pOptions" />.</param>
        public AuthnRequestFactory(TimeProvider time, IOptions<Saml2pOptions> options)
        {
            _time = time;
            _options = options.Value;
        }
#endif

        /// <summary>
        /// Creates an instance of <see cref="AuthnRequest"/>.
        /// </summary>
        /// <param name="context">The current <see cref="HttpContext"/>.</param>
        /// <param name="idp">The <see cref="ISaml2pIdentityProvider"/> to create the <see cref="AuthnRequest"/> for.</param>
        /// <returns>An awaitable <see cref="Task{TResult}"/> of type <see cref="AuthnRequest"/>.</returns>
        public async Task<AuthnRequest> CreateAuthnRequestAsync(HttpContext context, ISaml2pIdentityProvider idp)
        {
            var request = new AuthnRequest
            {
                Id = $"_{Guid.NewGuid()}",
                // TODO: have some sort of providername default
                ProviderName = idp.ExpectedIssuer ?? _options.DefaultIssuer,
                AssertionConsumerServiceUrl = GetAcsUrl(context.Request),
                IssueInstant = GetUtcNow(),
                Issuer = idp.ExpectedIssuer ?? _options.DefaultIssuer,
                Destination = new Uri(idp.BaseUrl, idp.AcceptSsoEndpoint),
                NameIdPolicy = new NameIdPolicy
                {
                    Format = idp.NameIdPolicyFormat
                },
                RequestedAuthnContext = new RequestedAuthnContext
                {
                    AuthnContextClassRef = idp.RequestedAuthnContextClassRef,
                    Comparison = idp.RequestedAuthnContextClassRefComparison
                }
            };

            if (idp.RequiresSignedAuthnRequest)
            {
                if(idp is { AuthnRequestSigningKey: not null, AuthnRequestSigningMethod: not null })
                    request.SigningCredentials = idp.AuthnRequestSigningMethod.CreateCredentials(idp.AuthnRequestSigningKey);
                else
                    throw new InvalidOperationException($"Partner '{idp.Id}' requires a signed AuthnRequest, but has misconfigured signing credentials.");
            }
            
            var generateContext = new GenerateRelayStateContext
            {
                Partner = idp,
                PartnerId = idp.Id,
                Request = request
            };

            await Events.InvokeAsync(_options, idp, e => e.OnGeneratingRelayState(context.RequestServices, generateContext));

            if (request.RelayState == null)
                request.RelayState = Guid.NewGuid().ToString();

            return request;
        }

        private Uri GetAcsUrl(HttpRequest request)
        {
            var baseUrl = new Uri($"{request.Scheme}://{request.Host}");

            // TODO: add central utility for creating all paths
            var path = request.PathBase.Add(_options.FinishPath);
            return new Uri(baseUrl, path);
        }

        private DateTime GetUtcNow()
        {
#if NET6_0
            return _systemClock.UtcNow.UtcDateTime;
#else
            return _time.GetUtcNow().UtcDateTime;
#endif
        }
    }
}
