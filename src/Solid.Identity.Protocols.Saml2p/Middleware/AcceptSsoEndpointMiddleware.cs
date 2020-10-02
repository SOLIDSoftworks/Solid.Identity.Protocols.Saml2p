using Microsoft.AspNetCore.Http;
using Solid.Identity.Protocols.Saml2p.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Solid.Identity.Protocols.Saml2p.Serialization;
using System.Linq;
using System.Security;
using Solid.Identity.Protocols.Saml2p.Cache;
using Solid.Identity.Protocols.Saml2p.Models.Context;

namespace Solid.Identity.Protocols.Saml2p.Middleware
{
    internal class AcceptSsoEndpointMiddleware : IdentityProviderEndpointMiddleware<AuthnRequest>
    {
        private Saml2pSerializer _serializer;

        public AcceptSsoEndpointMiddleware(Saml2pSerializer serializer, string idpId, Saml2pCache cache, IOptionsMonitor<Saml2pIdentityProviderOptions> monitor, ILoggerFactory loggerFactory, RequestDelegate next) 
            : base(idpId, cache, monitor, loggerFactory, next)
        {
            _serializer = serializer;
        }

        protected override async ValueTask HandleRequestAsync(HttpContext context, AuthnRequest request)
        {
            Logger.LogInformation("Accepting SAML2P authentication.");
            var partnerId = request?.Issuer;
            var partner = IdentityProvider.ServiceProviders.FirstOrDefault(sp => sp.Id == partnerId);

            if (partner == null)
                throw new SecurityException($"Partner '{partnerId}' not found.");

            if (!partner.Enabled)
                throw new SecurityException($"Partner '{partnerId}' is disabled.");

            await Cache.CacheRequestAsync(request.Id, request);

            var ssoContext = new AcceptSsoContext
            {
                PartnerId = partner.Id,
                Partner = partner,
                Request = request,
                User = context.User,
                ReturnUrl = GenerateReturnUrl(context, request.Id)
            };
            await IdentityProvider.Events.AcceptSsoAsync(context.RequestServices, ssoContext);
        }

        protected override bool IsValidRequest(HttpContext context, out AuthnRequest request)
        {   
            var base64 = context.Request.Form["SAMLRequest"].ToString();            
            var xml = Encoding.UTF8.GetString(Convert.FromBase64String(base64));
            request = _serializer.DeserializeAuthnRequest(xml);
            return request != null;
        }
    }
}
