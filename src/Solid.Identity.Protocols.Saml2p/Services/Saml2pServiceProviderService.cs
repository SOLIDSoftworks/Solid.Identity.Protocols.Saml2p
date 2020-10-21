//using Microsoft.AspNetCore.Http;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.IdentityModel.Tokens.Saml2;
//using Solid.Identity.Protocols.Saml2p.Abstractions;
//using Solid.Identity.Protocols.Saml2p.Areas.__Saml2p.Pages;
//using Solid.Identity.Protocols.Saml2p.Cache;
//using Solid.Identity.Protocols.Saml2p.Factories;
//using Solid.Identity.Protocols.Saml2p.Models.Context;
//using Solid.Identity.Protocols.Saml2p.Models.Protocol;
//using Solid.Identity.Protocols.Saml2p.Providers;
//using Solid.Identity.Protocols.Saml2p.Serialization;
//using System;
//using System.Collections.Generic;
//using System.Security.Claims;
//using System.Text;
//using System.Threading.Tasks;

//namespace Solid.Identity.Protocols.Saml2p.Services
//{
//    public class Saml2pServiceProviderService : ISaml2pServiceProviderService
//    {
//        private Saml2pPartnerProvider _partnerProvider;
//        private AuthnRequestFactory _authnRequestFactory;
//        private TokenValidationParametersFactory _tokenValidationParametersFactory;
//        private Saml2pSerializer _serializer;
//        private Saml2SecurityTokenHandler _handler;
//        private Saml2pCache _cache;
//        private IHttpContextAccessor _httpContextAccessor;
//        private IRazorPageRenderingService _razorPageRenderingService;

//        public Saml2pServiceProviderService(
//            Saml2pPartnerProvider partners, 
//            AuthnRequestFactory authnRequestFactory,
//            TokenValidationParametersFactory tokenValidationParametersFactory,
//            Saml2pSerializer serializer,
//            Saml2SecurityTokenHandler handler,
//            Saml2pCache cache,
//            IHttpContextAccessor httpContextAccessor,
//            IRazorPageRenderingService razorPageRenderingService
            
//        )
//        {
//            _partnerProvider = partners;
//            _authnRequestFactory = authnRequestFactory;
//            _tokenValidationParametersFactory = tokenValidationParametersFactory;
//            _serializer = serializer;
//            _handler = handler;
//            _cache = cache;
//            _httpContextAccessor = httpContextAccessor;
//            _razorPageRenderingService = razorPageRenderingService;
//        }

//        public async Task StartSsoAsync(string partnerId)
//        {
//            //var httpContext = _httpContextAccessor.HttpContext;
//            //var partner = await _partnerProvider.GetIdentityProviderAsync(partnerId);
//            //var request = _authnRequestFactory.CreateAuthnRequest(httpContext, partner);
//            //await _cache.CacheRequestAsync(request.Id, request);
//            //var context = new StartSsoContext
//            //{
//            //    PartnerId = partnerId,
//            //    Partner = partner, 
//            //    AuthnRequest = request
//            //};
//            //await partner.ServiceProvider.Events.StartSsoAsync(_provider, context);
//            //var xml = _serializer.SerializeAuthnRequest(request);
//            //var base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(xml));
//            //var model = new AuthnRequestModel
//            //{
//            //    Destination = new Uri(partner.BaseUrl, partner.SsoEndpoint),
//            //    SamlRequest = base64
//            //};
//            //var html = await _razorPageRenderingService.RenderPageAsync(model, "AuthnRequest", "__Saml2p");
//            //httpContext.Response.StatusCode = 200;
//            //httpContext.Response.ContentType = "text/html";
//            //var bytes = Encoding.UTF8.GetBytes(html);
//            //await httpContext.Response.Body.WriteAsync(bytes, 0, bytes.Length);
//        }

//        public async Task<ClaimsPrincipal> FinishSsoAsync()
//        {
//            //var httpContext = _httpContextAccessor.HttpContext;
//            //var base64 = httpContext.Request.Form["SAMLResponse"].ToString();
//            //var xml = Encoding.UTF8.GetString(Convert.FromBase64String(base64));
//            //var response = _serializer.DeserializeSamlResponse(xml);
//            //var partner = _partnerProvider.GetPartnerIdentityProvider(response.Issuer);
//            //var request = null as AuthnRequest;
//            //if (response.InResponseTo != null)
//            //    request = await _cache.FetchRequestAsync(response.InResponseTo);
//            //if (request == null && !partner.CanInitiateSso)
//            //    throw new Exception($"IDP partner {partner.Id} is not allowed to initiate SSO"); // better exception
//            //var completeContext = new FinishSsoContext
//            //{
//            //    PartnerId = partner.Id,
//            //    Partner = partner,
//            //    Request = request,
//            //    Response = response
//            //};
//            //await partner.ServiceProvider.Events.FinishSsoAsync(_provider, completeContext);

//            //var parameters = _tokenValidationParametersFactory.Create(partner);
//            //var validatedContext = new ValidateTokenContext
//            //{
//            //    PartnerId = partner.Id,
//            //    Partner = partner,
//            //    Request = request,
//            //    Response = response,
//            //    TokenValidationParameters = parameters,
//            //    Handler = _handler
//            //};
//            //var subject = await partner.ServiceProvider.Events.ValidateTokenAsync(_provider, validatedContext);
//            //return subject;
//        }
//    }
//}
