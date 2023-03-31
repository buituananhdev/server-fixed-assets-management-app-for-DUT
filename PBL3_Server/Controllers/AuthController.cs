using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using PBL3_Server.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BCrypt.Net;

namespace PBL3_Server.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly DataContext _dbContext;
        private readonly IConfiguration _configuration;
        public AuthController(DataContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }
        public static User user = new User();
        

        [HttpPost("login")]
        public IActionResult Login(User request)
        {
            // Kiểm tra thông tin đăng nhập của người dùng từ database
            var user = _dbContext.Users.SingleOrDefault(u => u.Username == request.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.Password))
            {
                return BadRequest(new {status = "failure", message = "Incorrect username or password!"});
            }

            // Tạo token và refresh token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.UserRole)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = creds
            };
            var jwtToken = tokenHandler.CreateToken(tokenDescriptor);
            var refreshToken = Guid.NewGuid().ToString();

            // Lưu token và refresh token vào cơ sở dữ liệu
            var tokenEntity = new TokenEntity
            {
                Username = user.Username,
                JwtToken = tokenHandler.WriteToken(jwtToken),
                RefreshToken = refreshToken,
                ExpirationTime = DateTime.UtcNow.AddDays(1),
                AccessTime = DateTime.Now
            };
            _dbContext.Tokens.Add(tokenEntity);
            _dbContext.SaveChanges();

            // Trả về token và refresh token cho client

            return Ok(new { Token = tokenHandler.WriteToken(jwtToken), Refresh_Token = refreshToken, ExpirationTime = DateTime.UtcNow.AddDays(1), AccessTime = DateTime.Now, Username = user.Username, Role = user.UserRole });
        }

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Get the token from the request header
            var token = HttpContext.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new {status= "failure", message= "Invalid token"});
            }

            // Validate the token and extract the username
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));
            try
            {
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                var claimsPrincipal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
                var username = claimsPrincipal.Identity?.Name;
                if (string.IsNullOrEmpty(username))
                {
                    return BadRequest(new { status = "failure", message = "Invalid username" });
                }

                // Remove the token and refresh token from the database
                var tokenEntity = _dbContext.Tokens.FirstOrDefault(t => t.Username == username && t.JwtToken == token);
                if (tokenEntity != null)
                {
                    _dbContext.Tokens.Remove(tokenEntity);
                    _dbContext.SaveChanges();
                }

                return Ok(new { status = "success", message = "Successfully logged out" });
            }
            catch (Exception)
            {
                return BadRequest(new { status = "failure", message = "Invalid token" });
            }
        }

        [HttpPost("refresh")]
        public IActionResult RefreshToken(string refreshToken)
        {
            // Tìm kiếm tokenEntity trong database với refresh token được truyền vào
            var tokenEntity = _dbContext.Tokens.SingleOrDefault(t => t.RefreshToken == refreshToken);

            // Kiểm tra xem tokenEntity có tồn tại không
            if (tokenEntity == null)
            {
                return BadRequest(new { status = "failure", message = "Invalid refresh token!" });
            }

            // Kiểm tra xem token đã hết hạn chưa
            if (tokenEntity.ExpirationTime < DateTime.UtcNow)
            {
                return BadRequest(new { status = "failure", message = "Token has expired!" });
            }

            // Tạo một token mới và refresh token mới
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.UserRole)
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = creds
            };
            var jwtToken = tokenHandler.CreateToken(tokenDescriptor);
            var newRefreshToken = Guid.NewGuid().ToString();

            // Cập nhật token mới và refresh token mới trong database
            tokenEntity.JwtToken = tokenHandler.WriteToken(jwtToken);
            tokenEntity.RefreshToken = newRefreshToken;
            tokenEntity.ExpirationTime = DateTime.UtcNow.AddDays(1);
            tokenEntity.AccessTime = DateTime.Now;
            _dbContext.SaveChanges();

            // Trả về token mới và refresh token mới cho client
            return Ok(new { Token = tokenHandler.WriteToken(jwtToken), refresh_token = newRefreshToken, ExpirationTime = DateTime.UtcNow.AddDays(1), AccessTime = DateTime.Now, Username = tokenEntity.Username });
        }


        [HttpGet("user")]
        public async Task<IActionResult> GetUser()
        {
            return Ok("");
        }
    }
}
