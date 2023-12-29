using CafeApp.AppModels;
using CafeApp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CafeApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginWithAdminController : ControllerBase
    {
        private readonly CafeManagerDBContext _context;

        private readonly IConfiguration _configuration;

        public LoginWithAdminController(IConfiguration configuration, CafeManagerDBContext context)
        {
            _configuration = configuration;
            _context = context;
        }
        [HttpPost]
        [Route("Login")]
        public IActionResult Login([FromBody] UserModel _userData)
        {
            if (_userData != null)
            {
                var resultLoginCheck = _context.Accounts
                    .Where(e => e.Email == _userData.Email && e.Password == _userData.Password && e.PermissionId == "1")
                    .FirstOrDefault();

                if (resultLoginCheck == null)
                {
                    return BadRequest("Invalid Credentials");
                }
                else
                {
                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("UserId", resultLoginCheck.UserId),
                        new Claim("Email", resultLoginCheck.Email),
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        _configuration["Jwt:Issuer"],
                        _configuration["Jwt:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddMinutes(10),
                        signingCredentials: signIn);

                    var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

                    return Ok(new { AccessToken = accessToken, Message = "Login Success" });
                }
            }
            else
            {
                return BadRequest("No Data Posted");
            }
        }
    }
}
