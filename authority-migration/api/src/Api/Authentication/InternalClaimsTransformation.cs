using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Api.Authentication
{
    public class InternalClaimsTransformation : IClaimsTransformation
    {
        public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            var identity = new ClaimsIdentity();

            // Map unique identifier claims from the old authority to those used in the new one
            var legacyUniqueId = principal.FindFirst(ClaimTypes.OldAuthorityUserId)?.Value;
            if (legacyUniqueId != null)
            {
                identity.AddClaim(new Claim(ClaimTypes.UserId, legacyUniqueId));
            }

            // Map unique identifier claims from the new authority to those used in the old ones
            var newUniqueId = principal.FindFirst(ClaimTypes.UserId)?.Value;
            if (newUniqueId != null)
            {
                identity.AddClaim(new Claim(ClaimTypes.OldAuthorityUserId, newUniqueId));
            }

            principal.AddIdentity(identity);

            return Task.FromResult(principal);
        }
    }
}
