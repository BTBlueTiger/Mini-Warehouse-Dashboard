using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniWarehouse.Api.Data;
using MiniWarehouse.Api.Model;
using Microsoft.AspNetCore.Identity;
using MiniWarehouse.Api.Model.Dto;

namespace MiniWarehouse.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(DatabaseContext context, IPasswordHasher<User> passwordHasher) : ControllerBase
    {
        private readonly DatabaseContext _context = context;
            private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            return await _context.User.ToListAsync();
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/User/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/User
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("register")]
        public async Task<ActionResult<User>> PostUser([FromBody] UserRegisterDto  userDto)
        {
            var user = new User
            {
                Email = userDto.Email,
                PasswordHash = _passwordHasher.HashPassword(new User { Email = userDto.Email }, userDto.Password) // Hash the password before saving
            };

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.Id == id);
        }

        [NonAction]
        public bool ValidateUser(User user, string providedPassword)
        {
            if (user.PasswordHash == null)
            {
                return false;
            }
            // Verify the password
            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                providedPassword
            );
            return result == PasswordVerificationResult.Success;
        }
    }
}
