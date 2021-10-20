using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;

namespace Api.Tests.Infrastructure
{
    public class TestApiFactory : WebApplicationFactory<Startup>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var applicationPath = Path.GetFullPath(PlatformServices.Default.Application.ApplicationBasePath);
            builder.UseContentRoot(applicationPath);
            base.ConfigureWebHost(builder);
        }
    }
}