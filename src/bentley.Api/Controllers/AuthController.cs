using bentley.Api.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bentley.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly JwtTokenGenerator _tokenGenerator;

        public AuthController(JwtTokenGenerator tokenGenerator)
        {
            _tokenGenerator = tokenGenerator;
        }

        // Only for development!
        [HttpGet("token")]
        [AllowAnonymous]
        public IActionResult GetDevToken(string userId = "1234567890", string name = "John Doe")
        {
            var token = _tokenGenerator.GenerateToken(userId, name, expiryDays: 30);
            return Ok(new { token });
        }
    }
}
