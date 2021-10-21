using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.PlatformAbstractions;

namespace Api.Tests.Setup
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