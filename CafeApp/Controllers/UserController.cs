using System;
using System.Collections.Generic;
using System.Linq;
using CafeApp.AppModels;
using CafeApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CafeApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly CafeManagerDBContext _context;

        public UserController(CafeManagerDBContext context)
        {
            _context = context;
        }

        // GET: api/User
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetAll()
        {
            return _context.Users.ToList();
        }

        // GET: api/User/id
        [HttpGet("{id}")]
        public ActionResult<User> GetUser(string id)
        {
            var user = _context.Users.Find(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }


        // PUT: api/User/id
        [HttpPut("{id}")]
        public IActionResult PutUser(string id, [FromBody] UserUpdateRequest request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }

            var existingUser = _context.Users.Find(id);

            if (existingUser == null)
            {
                return NotFound();
            }

            existingUser.Id = request.Id;
            existingUser.Name = request.Name;
            existingUser.Phone = request.Phone;
            existingUser.Avatar = request.Avatar;

            try
            {
                _context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                return StatusCode(500, "Concurrency error occurred while updating the user.");
            }

            return NoContent();
        }

        // DELETE: api/User/id
        [HttpDelete("{id}")]
        public IActionResult DeleteUser(string id)
        {
            var user = _context.Users.Find(id);

            if (user == null)
            {
                return NotFound();
            }

            // Delete associated account first
            var account = _context.Accounts.FirstOrDefault(a => a.UserId == id);
            if (account != null)
            {
                _context.Accounts.Remove(account);
            }

            _context.Users.Remove(user);
            _context.SaveChanges();

            return NoContent();
        }


        private bool UserExists(string id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
