using Api.Tests.Clients;
using Api.Tests.Infrastructure;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace Api.Tests.AuthenticationTests
{
    public class RefreshTokenTests : IDisposable
    {
        private readonly TestApiFactory _apiFactory;

        public RefreshTokenTests()
        {
            _apiFactory = new TestApiFactory();
        }

        [Fact]
        public async Task Custom_tokens_are_issued_when_authenticated_via_identity_server()
        {
            var tokenClient = new IdentityServerTokenClient();
            var token = await tokenClient.GetToken();
            var client = _apiFactory.CreateClient();
            using var request = new HttpRequestMessage(HttpMethod.Post, "token/refresh");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            using var response = await client.SendAsync(request);
            response.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task Custom_tokens_are_issued_when_authenticated_via_firebase()
        {
            var tokenClient = new FirebaseTokenClient();
            var token = await tokenClient.GetToken();
            var client = _apiFactory.CreateClient();
            using var request = new HttpRequestMessage(HttpMethod.Post, "token/refresh");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            using var response = await client.SendAsync(request);
            response.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task Attempts_to_create_custom_token_fail_when_unauthenticated()
        {
            var client = _apiFactory.CreateClient();
            using var request = new HttpRequestMessage(HttpMethod.Post, "token/refresh");
            using var response = await client.SendAsync(request);
            response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        }

        public void Dispose()
        {
            _apiFactory?.Dispose();
        }
    }
}
