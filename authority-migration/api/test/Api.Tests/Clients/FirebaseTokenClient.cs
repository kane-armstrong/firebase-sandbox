using Api.Tests.Infrastructure;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Api.Tests.Clients
{
    public class FirebaseTokenClient
    {
        private static readonly HttpClient Client = new HttpClient();

        public async Task<string> GetToken()
        {
            var settings = TestConfiguration.Instance;
            var options = settings.GetSection("FirebaseTokenClient").Get<FirebaseTokenClientOptions>();

            var uri = $"https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword?key={options.ApiKey}";
            using var response = await Client.PostAsJsonAsync(uri, new
            {
                email = options.Email,
                password = options.Password,
                returnSecureToken = options.ReturnSecureToken
            });

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return (string)JObject.Parse(content)["idToken"];
        }

        public async Task<string> SigninWithCustomToken(string token)
        {
            var settings = TestConfiguration.Instance;
            var options = settings.GetSection("FirebaseTokenClient").Get<FirebaseTokenClientOptions>();

            var uri = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithCustomToken?key={options.ApiKey}";
            using var response = await Client.PostAsJsonAsync(uri, new
            {
                token = token,
                returnSecureToken = options.ReturnSecureToken
            });

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return (string)JObject.Parse(content)["idToken"];
        }
    }
}
