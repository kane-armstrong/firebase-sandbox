using Microsoft.AspNetCore.Mvc;
using System;
using Microsoft.AspNetCore.Authorization;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TimeController : ControllerBase
    {
        [Authorize]
        [HttpGet]
        public IActionResult Get() => Ok(DateTimeOffset.Now);

        [HttpGet("utc")]
        public IActionResult GetUtc() => Ok(DateTimeOffset.UtcNow);
    }
}
