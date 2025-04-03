using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Solid.Identity.Protocols.Saml2p.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Solid.Identity.Protocols.Saml2p.Services
{
    internal class RazorPageRenderingService
    {
        private IRazorViewEngine _engine;
        private IRazorPageActivator _activator;
        private ITempDataProvider _tempDataProvider;
        private IHttpContextAccessor _httpContextAccessor;

        public RazorPageRenderingService(
            IRazorViewEngine engine,
            IRazorPageActivator activator,
            ITempDataProvider tempDataProvider,

            IHttpContextAccessor httpContextAccessor
        )
        {
            _engine = engine;
            _activator = activator;
            _tempDataProvider = tempDataProvider;

            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> RenderPageAsync<T>(T model, string path, string area = null)
        {
            using var activity = Saml2pConstants.Tracing.Saml2p.CreateActivity(
                $"{nameof(RazorPageRenderingService)}.{nameof(RenderPageAsync)}", ActivityKind.Server);
            var actionContext = CreateActionContext(path, area ?? string.Empty);
            var page = _engine.FindPage(actionContext, path).Page as Page;
            if (page == null)
                throw new ArgumentException($"Unable to find page {path}");
            var view = CreateView(page);
            using (var writer = new StringWriter())
            {
                var viewContext = CreateViewContext(actionContext, view, model, writer);
                page.PageContext = new PageContext
                {
                    ViewData = viewContext.ViewData
                };
                page.ViewContext = viewContext;
                _activator.Activate(page, viewContext);
                await page.ExecuteAsync();
                var rendered = writer.ToString();
                return rendered;
            }
        }

        private ActionContext CreateActionContext(string path, string area)
        {
            var routeData = new RouteData();
            routeData.Values.Add("page", path);
            routeData.Values.Add("area", area);
            var actionContext =
                new ActionContext(
                    _httpContextAccessor.HttpContext,
                    routeData,
                    new ActionDescriptor { RouteValues = routeData.Values.ToDictionary(v => v.Key, v => v.Value.ToString()) }
                );
            return actionContext;
        }

        private RazorView CreateView(IRazorPage page)
        {
            var view = new RazorView(_engine,
                _activator,
                new List<IRazorPage>(),
                page,
                HtmlEncoder.Default,
                new DiagnosticListener("RazorPageRenderingService"));
            return view;
        }

        private ViewContext CreateViewContext<T>(ActionContext actionContext, RazorView view, T model, TextWriter writer)
        {
            var viewContext = new ViewContext(
                actionContext,
                view,
                new ViewDataDictionary<T>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
                {
                    Model = model
                },
                new TempDataDictionary(
                    _httpContextAccessor.HttpContext,
                    _tempDataProvider
                ),
                writer,
                new HtmlHelperOptions()
            );
            return viewContext;
        }
    }
}
