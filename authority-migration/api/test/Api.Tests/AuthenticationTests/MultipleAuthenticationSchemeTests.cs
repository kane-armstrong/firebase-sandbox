using Api.Tests.Setup;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace Api.Tests.AuthenticationTests
{
    [Collection(TestCollections.SharedFirebaseContextTests)]
    public class MultipleAuthenticationSchemeTests
    {
        private readonly TestFixture _fixture;

        public MultipleAuthenticationSchemeTests(TestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task Calls_to_authenticated_endpoints_work_when_given_a_valid_identity_server_token()
        {
            var token = await _fixture.CreateIdentityServerToken();
            using var request = new HttpRequestMessage(HttpMethod.Get, "time");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            using var response = await _fixture.Client.SendAsync(request);
            response.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task Calls_to_authenticated_endpoints_work_when_given_a_valid_firebase_token()
        {
            var token = await _fixture.CreateFirebaseToken();
            using var request = new HttpRequestMessage(HttpMethod.Get, "time");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            using var response = await _fixture.Client.SendAsync(request);
            response.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task Calls_to_authenticated_endpoints_work_when_given_a_valid_custom_token()
        {
            var token = await _fixture.CreateIdentityServerToken();
            using var createCustomTokenRequest = new HttpRequestMessage(HttpMethod.Post, "token/refresh");
            createCustomTokenRequest.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            using var createCustomTokenResponse = await _fixture.Client.SendAsync(createCustomTokenRequest);
            var customToken = await createCustomTokenResponse.Content.ReadAsStringAsync();

            var firebaseToken = await _fixture.FirebaseTokenClient.SigninWithCustomToken(customToken);
            using var request = new HttpRequestMessage(HttpMethod.Get, "time");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", firebaseToken);
            using var response = await _fixture.Client.SendAsync(request);
            response.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task Calls_to_authenticated_endpoints_401_when_no_token_is_provided()
        {
            using var response = await _fixture.Client.GetAsync("/time");
            response.StatusCode.Should().Be(StatusCodes.Status401Unauthorized);
        }

        [Fact]
        public async Task Calls_to_unauthenticated_endpoints_work_when_no_token_is_provided()
        {
            using var response = await _fixture.Client.GetAsync("/time/utc");
            response.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task Calls_to_unauthenticated_endpoints_work_when_given_a_valid_identity_server_token()
        {
            var token = await _fixture.CreateIdentityServerToken();
            using var request = new HttpRequestMessage(HttpMethod.Get, "time/utc");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            using var response = await _fixture.Client.SendAsync(request);
            response.StatusCode.Should().Be(StatusCodes.Status200OK);
        }

        [Fact]
        public async Task Calls_to_unauthenticated_endpoints_work_when_given_a_valid_firebase_token()
        {
            var token = await _fixture.CreateFirebaseToken();
            using var request = new HttpRequestMessage(HttpMethod.Get, "time/utc");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            using var response = await _fixture.Client.SendAsync(request);
            response.StatusCode.Should().Be(StatusCodes.Status200OK);
        }
    }
}