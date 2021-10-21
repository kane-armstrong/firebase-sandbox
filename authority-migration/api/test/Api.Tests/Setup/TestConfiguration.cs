using System.IO;
using Microsoft.Extensions.Configuration;

namespace Api.Tests.Setup
{
    public class TestConfiguration
    {
        private static IConfiguration _configuration;

        public static IConfiguration Instance
        {
            get
            {
                if (_configuration == null)
                {
                    BuildConfiguration();
                }
                return _configuration;
            }
        }

        private static void BuildConfiguration() =>
            _configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .AddEnvironmentVariables()
                .Build();
    }
}
