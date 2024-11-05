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
using Solid.Identity.Protocols.Saml2p.Cache;
using Solid.Identity.Protocols.Saml2p.Providers;
using Microsoft.IdentityModel.Tokens.Saml2;
using System.Linq;
using System.Security;
using Solid.Identity.Protocols.Saml2p.Abstractions;
using System.Security.Claims;
using Solid.Identity.Protocols.Saml2p.Models.Context;
using Solid.Identity.Protocols.Saml2p.Serialization;
using Solid.Identity.Protocols.Saml2p.Factories;
using Solid.Identity.Protocols.Saml2p.Areas.__Saml2p.Pages;
using Microsoft.Extensions.Primitives;
using Solid.Identity.Protocols.Saml2p.Services;
using Solid.Identity.Protocols.Saml2p.Models;
using System.Net;
using Microsoft.AspNetCore.Authentication;

namespace Solid.Identity.Protocols.Saml2p.Middleware.Idp
{
    internal class CompleteSsoEndpointMiddleware : Saml2pEndpointMiddleware
    {
        private RazorPageRenderingService _razor;
        private Saml2SecurityTokenHandler _handler;
        private ISecurityTokenDescriptorFactory _descriptorFactory;
        private SamlResponseFactory _responseFactory;

        public CompleteSsoEndpointMiddleware(
            RazorPageRenderingService razor, 
            Saml2SecurityTokenHandler handler, 
            ISecurityTokenDescriptorFactory descriptorFactory, 
            SamlResponseFactory responseFactory, 
            Saml2pSerializer serializer, 
            Saml2pCache cache, 
            Saml2pPartnerProvider partners,
            Saml2pEncodingService encoder,
            IOptionsMonitor<Saml2pOptions> monitor, 
            ILoggerFactory loggerFactory, 
            RequestDelegate _)
            : base(serializer, cache, partners, encoder, monitor, loggerFactory)
        {
            _razor = razor;
            _handler = handler;
            _descriptorFactory = descriptorFactory;
            _responseFactory = responseFactory;
        }

        public override async Task InvokeAsync(HttpContext context)
        {
            Logger.LogInformation("Completing SAML2P authentication (IDP flow).");
            var id = context.Request.Query["id"];
            if(StringValues.IsNullOrEmpty(id))
                throw new InvalidOperationException($"Missing 'id' query parameter.");

            var response = null as SamlResponse;

            var authentication = await context.AuthenticateAsync();
            var request = await Cache.FetchRequestAsync(id);
            if (request == null)
                throw new SecurityException($"SAMLRequest not found for id: '{id}'");
            
            Trace("Found cached SAMLRequest.", request);
            var partner = await Partners.GetServiceProviderAsync(request.Issuer);
            
            if (partner == null)
                throw new SecurityException($"Partner '{request.Issuer}' not found.");

            //if (!partner.Enabled)
            //    throw new SecurityException($"Partner '{partnerId}' is disabled.");

            var status = await Cache.FetchStatusAsync(id);
            await Cache.RemoveAsync(id);
            if (status != null)
            {
                Trace("Found cached Status.", request.RelayState, status);
                response = _responseFactory.Create(partner, status, authnRequestId: request.Id, relayState: request.RelayState);
            }
            else if (authentication.Succeeded && authentication.Principal?.Identity is ClaimsIdentity identity)
            {
                var descriptor = await _descriptorFactory.CreateSecurityTokenDescriptorAsync(identity, partner);
                var createSecurityTokenContext = new CreateSecurityTokenContext
                {
                    PartnerId = partner.Id,
                    Partner = partner,
                    TokenDescriptor = descriptor,
                    Handler = _handler
                };

                await Events.InvokeAsync(Options, partner, e => e.OnCreatingSecurityToken(context.RequestServices, createSecurityTokenContext));

                createSecurityTokenContext.SecurityToken ??=
                    createSecurityTokenContext.Handler.CreateToken(createSecurityTokenContext.TokenDescriptor) as Saml2SecurityToken;

                await Events.InvokeAsync(Options, partner, e => e.OnCreatedSecurityToken(context.RequestServices, createSecurityTokenContext));

                response = _responseFactory.Create(partner, authnRequestId: request.Id, relayState: request.RelayState, token: createSecurityTokenContext.SecurityToken);
            }
            else
            {
                response = _responseFactory.Create(partner, authnRequestId: request.Id, relayState: request.RelayState, status: SamlResponseStatus.Responder, subStatus: SamlResponseStatus.AuthnFailed);
            }
            
            var completeSsoContext = new CompleteSsoContext
            {
                PartnerId = request.Issuer,
                Partner = partner,
                Request = request,
                Response = response
            };
            await Events.InvokeAsync(Options, partner, e => e.OnCompleteSso(context.RequestServices, completeSsoContext));
            
            var binding = Convert(request.ProtocolBinding) ??  partner.SupportedBindings.First();
            Trace($"Sending SAMLResponse using {binding} binding.", response);

            var destination = request.AssertionConsumerServiceUrl ?? new Uri(partner.BaseUrl, partner.AssertionConsumerServiceEndpoint);
            await CompleteSsoAsync(context, response, destination, binding);
        }

        private BindingType? Convert(string protocolBinding)
        {
            if (protocolBinding == Saml2pConstants.Bindings.Post) return BindingType.Post;
            if (protocolBinding == Saml2pConstants.Bindings.Redirect) return BindingType.Redirect;

            return null;
        }

        private Task CompleteSsoAsync(HttpContext context, SamlResponse response, Uri destination, BindingType binding)
        {
            var base64 = SerializeSamlResponse(response, binding);
            if (binding == BindingType.Post) return PostAsync(context, base64, destination, response.RelayState);
            if (binding == BindingType.Redirect) return RedirectAsync(context, base64, destination, response.RelayState);

            throw new ArgumentException($"Unsupported binding type: '{binding}'");
        }

        private Task RedirectAsync(HttpContext context, string base64, Uri destination, string relayState)
        {
            var queryBuilder = new StringBuilder();
            if (string.IsNullOrEmpty(destination.Query))
                queryBuilder.Append("?");
            else
                queryBuilder.Append("&");
            queryBuilder.Append("SAMLResponse=");
            queryBuilder.Append(WebUtility.UrlEncode(base64));
            if (!string.IsNullOrEmpty(relayState))
            {
                queryBuilder.Append("&");
                queryBuilder.Append("RelayState=");
                queryBuilder.Append(WebUtility.UrlEncode(relayState));
            }

            var url = $"{destination}{queryBuilder.ToString()}";

            context.Response.Redirect(url, false);
            return Task.CompletedTask;
        }

        private async Task PostAsync(HttpContext context, string base64, Uri destination, string relayState)
        {
            var model = new SamlResponseModel
            {
                Recipient = destination,
                SamlResponse = base64,
                RelayState = relayState
            };
            var html = await _razor.RenderPageAsync(model, "SamlResponse", "__Saml2p");
            context.Response.StatusCode = 200;
            context.Response.ContentType = "text/html";
            var bytes = new UTF8Encoding(false).GetBytes(html);
            await context.Response.Body.WriteAsync(bytes, 0, bytes.Length);
        }
    }
}
