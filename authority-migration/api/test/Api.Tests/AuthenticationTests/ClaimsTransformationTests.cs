using Api.Authentication;
using FluentAssertions;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace Api.Tests.AuthenticationTests
{
    public class ClaimsTransformationTests
    {
        [Fact]
        public async Task Identity_server_unique_id_is_mapped_to_firebase_unique_id()
        {
            var sub = Guid.NewGuid().ToString();
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(Authentication.ClaimTypes.OldAuthorityUserId, sub));
            var principal = new ClaimsPrincipal(new ClaimsIdentity[] { identity });

            var sut = new InternalClaimsTransformation();
            var transformation = await sut.TransformAsync(principal);

            var result = transformation.FindFirst(Authentication.ClaimTypes.UserId);
            result.Should().NotBeNull();
            result.Value.Should().Be(sub);
        }

        [Fact]
        public async Task Firebase_unique_id_is_mapped_to_identity_server_unique_id()
        {
            var sub = Guid.NewGuid().ToString();
            var identity = new ClaimsIdentity();
            identity.AddClaim(new Claim(Authentication.ClaimTypes.OldAuthorityUserId, sub));
            var principal = new ClaimsPrincipal(new ClaimsIdentity[] { identity });

            var sut = new InternalClaimsTransformation();
            var transformation = await sut.TransformAsync(principal);

            var result = transformation.FindFirst(Authentication.ClaimTypes.UserId);
            result.Should().NotBeNull();
            result.Value.Should().Be(sub);
        }
    }
}
