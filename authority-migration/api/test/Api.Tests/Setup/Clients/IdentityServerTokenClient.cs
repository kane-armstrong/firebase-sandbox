using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Api.Tests.Setup.Clients
{
    public class IdentityServerTokenClient : IDisposable
    {
        private readonly IdentityServerTokenClientOptions _options;
        private readonly HttpClient _client;

        public IdentityServerTokenClient(IdentityServerTokenClientOptions options, HttpClient client)
        {
            _options = options;
            _client = client;
        }

        public async Task<string> GetToken()
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, $"{_options.Host}/connect/token")
            {
                Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", _options.GrantType),
                    new KeyValuePair<string, string>("client_id", _options.ClientId),
                    new KeyValuePair<string, string>("client_secret", _options.ClientSecret),
                    new KeyValuePair<string, string>("scope", _options.Scope),
                    new KeyValuePair<string, string>("username", _options.UserName),
                    new KeyValuePair<string, string>("password", _options.Password)
                })
            };

            using var response = await _client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return (string)JObject.Parse(content)["access_token"];
        }

        public void Dispose()
        {
            _client?.Dispose();
        }
    }
}
