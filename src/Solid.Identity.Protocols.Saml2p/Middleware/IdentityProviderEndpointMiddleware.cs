using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Solid.Identity.Protocols.Saml2p.Cache;
using Solid.Identity.Protocols.Saml2p.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.Saml2p.Middleware
{
    internal abstract class IdentityProviderEndpointMiddleware<TModel> : IDisposable
    {
        private IDisposable _optionsChangeToken;

        protected IdentityProviderEndpointMiddleware(string idpId, Saml2pCache cache, IOptionsMonitor<Saml2pIdentityProviderOptions> monitor, ILoggerFactory loggerFactory, RequestDelegate next)
        {
            Next = next;
            Cache = cache;
            Logger = loggerFactory.CreateLogger(GetType());
            IdentityProvider = monitor.Get(idpId);
            _optionsChangeToken = monitor.OnChange((options, name) =>
            {
                if (name == idpId) 
                    IdentityProvider = options;
            });
        }

        protected RequestDelegate Next { get; }
        protected Saml2pCache Cache { get; }
        protected ILogger Logger { get; }
        protected Saml2pIdentityProviderOptions IdentityProvider { get; private set; }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!IdentityProvider.Enabled)
            {
                Logger.LogWarning($"SAML2P request attempted on disabled IDP: {IdentityProvider.Id}");
                context.Response.StatusCode = 400;
                return;
            }
            if (!IdentityProvider.ServiceProviders.Any(sp => sp.Enabled))
            {
                Logger.LogWarning($"SAML2P request attempted on IDP with no enabled SPs: {IdentityProvider.Id}");
                context.Response.StatusCode = 400;
                return;
            }
            if (IsValidRequest(context, out var model))
            {
                using (Logger.BeginScope("Handling SAML2P request."))
                    await HandleRequestAsync(context, model);
            }
            else
            {
                Logger.LogInformation($"Could not handle SAML2P request.");
                context.Response.StatusCode = 400;
                return;
            }
        }

        protected abstract bool IsValidRequest(HttpContext context, out TModel model);

        protected abstract ValueTask HandleRequestAsync(HttpContext context, TModel model); 
        
        protected string GenerateReturnUrl(HttpContext httpContext, string id)
        {
            var request = httpContext.Request;
            // PathBase is only used because we set the path prefix as pathbase in Startup
            return $"{request.PathBase}/complete?id={id}";
        }

        public void Dispose() => _optionsChangeToken?.Dispose();
    }
}
