using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Api.Tests.Setup.Clients
{
    public class FirebaseTokenClient : IDisposable
    {
        private readonly FirebaseTokenClientOptions _options;
        private readonly HttpClient _client;

        public FirebaseTokenClient(FirebaseTokenClientOptions options, HttpClient client)
        {
            _options = options;
            _client = client;
        }

        public async Task<string> GetToken()
        {
            var uri = $"https://www.googleapis.com/identitytoolkit/v3/relyingparty/verifyPassword?key={_options.ApiKey}";
            using var response = await _client.PostAsJsonAsync(uri, new
            {
                email = _options.Email,
                password = _options.Password,
                returnSecureToken = _options.ReturnSecureToken
            });

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return (string)JObject.Parse(content)["idToken"];
        }

        public async Task<string> SigninWithCustomToken(string token)
        {
            var uri = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithCustomToken?key={_options.ApiKey}";
            using var response = await _client.PostAsJsonAsync(uri, new
            {
                token = token,
                returnSecureToken = _options.ReturnSecureToken
            });

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return (string)JObject.Parse(content)["idToken"];
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
