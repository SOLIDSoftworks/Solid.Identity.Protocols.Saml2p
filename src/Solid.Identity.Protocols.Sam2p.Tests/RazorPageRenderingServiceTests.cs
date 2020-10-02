using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.ObjectPool;
using Moq;
using Solid.Identity.Protocols.Saml2p.Abstractions;
using Solid.Identity.Protocols.Saml2p.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Utilities.Razor.Tests.Pages;
using Xunit;

namespace Solid.Identity.Protocols.Saml2p.Tests
{
    public class RazorPageRenderingServiceTests
    {
        private ServiceProvider _root;
        private HttpContext _context;

        public RazorPageRenderingServiceTests()
        {
            _context = new DefaultHttpContext
            {

            };
            var mockHostingEnvironment = new Mock<IWebHostEnvironment>();
            var mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            mockHttpContextAccessor.Setup(a => a.HttpContext).Returns(() => _context);
            var services = new ServiceCollection();
            services
                .AddSingleton<IHttpContextAccessor>(mockHttpContextAccessor.Object)
                .AddSingleton<DiagnosticSource>(new DiagnosticListener("RazorPageRenderingServiceTests"))
                .AddSingleton<IWebHostEnvironment>(mockHostingEnvironment.Object)
                .AddLogging()
                .AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>()
                .AddScoped<RazorPageRenderingService>()
                .AddMvc()
                .AddRazorRuntimeCompilation(options =>
                {
                    options.FileProviders.Add(new EmbeddedFileProvider(typeof(RazorPageRenderingServiceTests).Assembly));
                })
            ;

            _root = services.BuildServiceProvider();
        }

        //[Theory]
        //[InlineData("Some page", "Some text")]
        public async Task ShouldRenderRazorPage(string title, string text)
        {
            var expected = $"<html><head><title>{title}</title></head><body>{text}</body></html>";
            using (var scope = _root.CreateScope())
            {
                var provider = scope.ServiceProvider;
                var service = provider.GetService<RazorPageRenderingService>();
                var html = await service.RenderPageAsync<TestPageModel>(new TestPageModel { Title = title, Text = text }, "TestPage");
                Assert.Equal(expected, html);
            }
        }
    }
}
