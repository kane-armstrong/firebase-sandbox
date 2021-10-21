using System;
using System.Net.Http;
using System.Threading.Tasks;
using Api.Tests.Setup.Clients;
using Microsoft.Extensions.Configuration;

namespace Api.Tests.Setup
{
    public class TestFixture : IDisposable
    {
        private readonly TestApiFactory _testApiFactory;

        public HttpClient Client { get; }
        public FirebaseTokenClient FirebaseTokenClient { get; }
        public IdentityServerTokenClient IdentityServerTokenClient { get; }

        public TestFixture()
        {
            _testApiFactory = new TestApiFactory();
            Client = _testApiFactory.CreateClient();

            var settings = TestConfiguration.Instance;

            var firebaseOptions = settings.GetSection("FirebaseTokenClient").Get<FirebaseTokenClientOptions>();
            FirebaseTokenClient = new FirebaseTokenClient(firebaseOptions, new HttpClient());

            var identityServerOptions = settings.GetSection("IdentityServerTokenClient").Get<IdentityServerTokenClientOptions>();
            IdentityServerTokenClient = new IdentityServerTokenClient(identityServerOptions, new HttpClient());
        }

        public async Task<string> CreateFirebaseToken()
        {
            return await FirebaseTokenClient.GetToken();
        }

        public async Task<string> CreateIdentityServerToken()
        {
            return await IdentityServerTokenClient.GetToken();
        }

        public void Dispose()
        {
            _testApiFactory?.Dispose();
            Client?.Dispose();
        }
    }
}
