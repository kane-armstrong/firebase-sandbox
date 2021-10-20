using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Api.Tests.Infrastructure;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace Api.Tests.Clients
{
    public class IdentityServerTokenClient
    {
        private static readonly HttpClient Client = new HttpClient();

        public async Task<string> GetToken()
        {
            var settings = TestConfiguration.Instance;
            var options = settings.GetSection("IdentityServerTokenClient").Get<IdentityServerTokenClientOptions>();

            using var request = new HttpRequestMessage(HttpMethod.Post, $"{options.Host}/connect/token")
            {
                Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", options.GrantType),
                    new KeyValuePair<string, string>("client_id", options.ClientId),
                    new KeyValuePair<string, string>("client_secret", options.ClientSecret),
                    new KeyValuePair<string, string>("scope", options.Scope),
                    new KeyValuePair<string, string>("username", options.UserName),
                    new KeyValuePair<string, string>("password", options.Password)
                })
            };

            using var response = await Client.SendAsync(request);

            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            return (string)JObject.Parse(content)["access_token"];
        }
    }
}
