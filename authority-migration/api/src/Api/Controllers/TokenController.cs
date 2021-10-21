using Api.Configuration;
using FirebaseAdmin.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Api.Authentication;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly FirebaseAuth _firebase;

        public TokenController(FirebaseAuth firebase)
        {
            _firebase = firebase;
        }

        [Authorize]
        [HttpPost("refresh")]
        public async Task<IActionResult> CreateCustomToken()
        {
            var uid = User.FindFirst(ClaimTypes.UserId).Value;
            var token = await _firebase.CreateCustomTokenAsync(uid);
            return Ok(token);
        }
    }
}
