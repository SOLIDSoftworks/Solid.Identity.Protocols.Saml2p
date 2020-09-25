using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.Saml2p.Abstractions.Services
{
    public interface IRazorPageRenderingService
    {
        Task<string> RenderPageAsync<T>(T model, string path, string area = null);
    }
}
