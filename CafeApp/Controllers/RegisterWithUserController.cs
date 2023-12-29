using CafeApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CafeApp.Controllers
{
    public class RegisterDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string? Phone { get; set; }
        public string? Avatar { get; set; }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterWithUserController : ControllerBase
    {
        private readonly CafeManagerDBContext _dbContext;

        public RegisterWithUserController(CafeManagerDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid model state");
            }

            if (await _dbContext.Accounts.AnyAsync(a => a.Email == registerDto.Email))
            {
                return BadRequest("User already exists");
            }

            var user = new User
            {
                Id = GenerateUniqueId(_dbContext.Users),
                Name = registerDto.Name,
                Phone = registerDto.Phone,
                Avatar = registerDto.Avatar
            };

            var account = new Account
            {
                Id = GenerateUniqueId(_dbContext.Accounts),
                Email = registerDto.Email,
                Password = registerDto.Password,
                UserId = user.Id,
                PermissionId = "2"
            };

            _dbContext.Users.Add(user);
            _dbContext.Accounts.Add(account);

            await _dbContext.SaveChangesAsync();

            return Ok("Registration successful");
        }

        private string GenerateUniqueId<T>(DbSet<T> set) where T : class
        {
            string newId;
            do
            {
                newId = Guid.NewGuid().ToString("N").Substring(0, 20);
            } while (set.Any(e => EF.Property<string>(e, "Id") == newId));

            return newId;
        }
    }
}
