using Microsoft.AspNetCore.Http;
using Solid.Identity.Protocols.Saml2p.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Solid.Extensions.AspNetCore.Saml2p.Middleware
{
    public class AcceptSsoEndpointMiddleware
    {
        private RequestDelegate _next;

        public AcceptSsoEndpointMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await context.AcceptSsoAsync();
        }
    }
}
