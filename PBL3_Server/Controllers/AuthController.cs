using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using PBL3_Server.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PBL3_Server.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static User user = new User();

        private readonly IConfiguration _configuration;
        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private readonly List<User> _users = new List<User>
        {
            new User { Username = "admin", Password = "123456" }
        };

        [HttpPost("login")]
        public ActionResult<User> Login(User request)
        {
            var existingUser = _users.FirstOrDefault(u => u.Username == request.Username && u.Password == request.Password);
            if (existingUser == null)
            {
                return BadRequest("Invalid username or password");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("role", "admin") }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = creds
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var authResponse = new AuthResponse { Token = tokenHandler.WriteToken(token), Role = "admin" };
            return Ok(authResponse);
        }

        [HttpGet("user")]
        public async Task<IActionResult> GetUser()
        {
            return Ok("");
        }
    }
}
