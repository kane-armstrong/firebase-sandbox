using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Api.Tests.Setup;
using Xunit;

namespace Api.Tests.AuthenticationTests
{
    [Collection(TestCollections.SharedFirebaseContextTests)]
    public class RefreshTokenTests
    {
        private readonly TestFixture _fixture;

        public RefreshTokenTests(TestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Custom_tokens_are_issued_when_authenticated_via_identity_server()
        {
            var token = await _fixture.CreateIdentityServerToken();
            using var request = new HttpRequestMessage(HttpMethod.Post, "token/refresh");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            using var response = await _fixture.Client.SendAsync(request);
            response.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task Custom_tokens_are_issued_when_authenticated_via_firebase()
        {
            var token = await _fixture.CreateFirebaseToken();
            using var request = new HttpRequestMessage(HttpMethod.Post, "token/refresh");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            using var response = await _fixture.Client.SendAsync(request);
            response.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task Attempts_to_create_custom_token_fail_when_unauthenticated()
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "token/refresh");
            using var response = await _fixture.Client.SendAsync(request);
            response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        }
    }
}
