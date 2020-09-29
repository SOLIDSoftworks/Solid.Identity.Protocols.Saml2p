using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens.Saml2;
using Solid.Identity.Protocols.Saml2p.Abstractions.Factories;
using Solid.Identity.Protocols.Saml2p.Abstractions.Services;
using Solid.Identity.Protocols.Saml2p.Areas.__Saml2p.Pages;
using Solid.Identity.Protocols.Saml2p.Cache;
using Solid.Identity.Protocols.Saml2p.Factories;
using Solid.Identity.Protocols.Saml2p.Models.Context;
using Solid.Identity.Protocols.Saml2p.Models.Protocol;
using Solid.Identity.Protocols.Saml2p.Providers;
using Solid.Identity.Protocols.Saml2p.Serialization;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.Saml2p.Services
{
    public class Saml2pIdentityProviderService : ISaml2pIdentityProviderService
    {
        private IServiceProvider _provider;
        private Saml2pPartnerProvider _partnerProvider;
        private Saml2pSerializer _serializer;
        private IRazorPageRenderingService _razorPageRenderingService;
        private Saml2SecurityTokenHandler _handler;
        private Saml2pCache _cache;
        private SamlResponseFactory _factory;
        private ISecurityTokenDescriptorFactory _securityTokenDescriptorFactory;
        private IHttpContextAccessor _httpContextAccessor;

        public Saml2pIdentityProviderService(
            IServiceProvider provider,
            Saml2pPartnerProvider partnerProvider,
            Saml2pSerializer serializer,
            Saml2SecurityTokenHandler handler,
            Saml2pCache cache,
            SamlResponseFactory factory,
            ISecurityTokenDescriptorFactory securityTokenDescriptorFactory,
            IHttpContextAccessor httpContextAccessor,
            IRazorPageRenderingService razorPageRenderingService
        )
        {
            _provider = provider;
            _partnerProvider = partnerProvider;
            _serializer = serializer;
            _handler = handler;
            _cache = cache;
            _factory = factory;
            _securityTokenDescriptorFactory = securityTokenDescriptorFactory;
            _httpContextAccessor = httpContextAccessor;
            _razorPageRenderingService = razorPageRenderingService;
        }

        public async Task AcceptSsoAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (!HttpMethods.IsPost(httpContext.Request.Method)) return;

            var base64 = httpContext.Request.Form["SAMLRequest"].ToString();
            var xml = Encoding.UTF8.GetString(Convert.FromBase64String(base64));
            var request = _serializer.DeserializeAuthnRequest(xml);
            if (request == null) return;

            var partner = _partnerProvider.GetPartnerServiceProvider(request.Issuer);
            // TODO: If partner doesn't exist, then respond with correct status

            if (!partner.Enabled || !partner.IdentityProvider.Enabled) return;

            await _cache.CacheRequestAsync(request.Id, request);

            var context = new AcceptSsoContext
            {
                PartnerId = partner.Id,
                Partner = partner,
                Request = request,
                User = httpContext.User,
                ReturnUrl = GenerateReturnUrl(httpContext, request.Id)
            };
            await partner.IdentityProvider.Events.AcceptSsoAsync(_provider, context);
        }

        public async Task InitiateSsoAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (!HttpMethods.IsGet(httpContext.Request.Method)) return;
            var partnerId = httpContext.Request.Query["partnerId"];

            var partner = _partnerProvider.GetPartnerServiceProvider(partnerId);
            // TODO: If partner doesn't exist, then respond with correct status

            if (!partner.Enabled || !partner.IdentityProvider.Enabled) return;

            var request = new AuthnRequest
            {
                AssertionConsumerServiceUrl = new Uri(partner.BaseUrl, partner.AssertionConsumerServiceEndpoint),
                Issuer = partner.Id                
            };
            var key = $"idp_initiated_{Guid.NewGuid().ToString()}";
            await _cache.CacheRequestAsync(key, request);

            var context = new InitiateSsoContext
            {
                PartnerId = partner.Id,
                Partner = partner,
                User = httpContext.User,
                ReturnUrl = GenerateReturnUrl(httpContext, key)
            };
            await partner.IdentityProvider.Events.InitiateSsoAsync(_provider, context);
        }

        public async Task CompleteSsoAsync()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            if (!HttpMethods.IsGet(httpContext.Request.Method)) return;
            var user = httpContext.User;
            var id = httpContext.Request.Query["id"];
            var request = await _cache.FetchRequestAsync(id);
            var partner = _partnerProvider.GetPartnerServiceProvider(request.Issuer);

            if (user.Identity.IsAuthenticated)
            {
                var descriptor = _securityTokenDescriptorFactory.CreateSecurityTokenDescriptor(user.Identity as ClaimsIdentity, partner);
                var createSecurityTokenContext = new CreateSecurityTokenContext
                {
                    PartnerId = partner.Id,
                    Partner = partner,
                    TokenDescriptor = descriptor
                };
                await partner.IdentityProvider.Events.CreateSecurityTokenAsync(_provider, createSecurityTokenContext);
                var token = _handler.CreateToken(descriptor) as Saml2SecurityToken;
                var response = _factory.Create(partner, authnRequestId: request.Id, token: token);
                var xml = _serializer.SerializeSamlResponse(response);
                var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(xml));
                var model = new SamlResponseModel
                {
                    Recipient = new Uri(partner.BaseUrl, partner.AssertionConsumerServiceEndpoint),
                    SamlResponse = base64,
                    // TODO: RelayState = 
                };
                var html = await _razorPageRenderingService.RenderPageAsync(model, "SamlResponse", "__Saml2p");
                httpContext.Response.StatusCode = 200;
                httpContext.Response.ContentType = "text/html";
                var bytes = Encoding.UTF8.GetBytes(html);
                await httpContext.Response.Body.WriteAsync(bytes, 0, bytes.Length);
            }

        }        

        //private AuthnRequest GetAuthnRequest(HttpContext context)
        //{
        //    if (!HttpMethods.IsPost(context.Request.Method)) return null;

        //    context.Request.EnableRewind();
        //    var request = _serializer.DeserializeAuthnRequest(context.Request.Body);
        //    if (request == null)
        //        context.Request.Body.Position = 0;
        //    return request;
        //}

        private string GenerateReturnUrl(HttpContext httpContext, string id)
        {
            var request = httpContext.Request;
            return $"{request.PathBase}{request.Path}/complete?id={id}";
        }
    }
}
