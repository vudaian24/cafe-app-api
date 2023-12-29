using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using CafeApp.AppModels;
using CafeApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CafeApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginWithUserController : ControllerBase
    {
        private readonly CafeManagerDBContext _context;
        private readonly IConfiguration _configuration;

        public LoginWithUserController(IConfiguration configuration, CafeManagerDBContext context)
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
                    .Include(a => a.User)
                    .Where(e => e.Email == _userData.Email && e.Password == _userData.Password && e.PermissionId == "2")
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
                        new Claim("UserName", resultLoginCheck.User.Name),  // Additional user information
                        new Claim("UserPhone", resultLoginCheck.User.Phone ?? ""),  // Additional user information
                        new Claim("UserAvatar", resultLoginCheck.User.Avatar ?? ""),  // Additional user information
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

                    return Ok(new
                    {
                        AccessToken = accessToken,
                        UserId = resultLoginCheck.UserId,
                        UserName = resultLoginCheck.User.Name,
                        UserPhone = resultLoginCheck.User.Phone ?? "",
                        UserAvatar = resultLoginCheck.User.Avatar ?? "",
                        Message = "Login Success"
                    });
                }
            }
            else
            {
                return BadRequest("No Data Posted");
            }
        }
    }
}
