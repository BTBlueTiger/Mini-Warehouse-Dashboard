using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniWarehouse.Api.Data;
using MiniWarehouse.Shared.Model;
using MiniWarehouse.Shared.Dto;
using Microsoft.AspNetCore.Identity;
using MiniWarehouse.Api.IService;
using MiniWarehouse.Api.Service;

namespace MiniWarehouse.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService, IAuthService authService) : ControllerBase
    {
        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            return Ok(await userService.GetUsersAsync());
        }

        // GET: api/User/5
        [HttpGet("byId/{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await userService.GetUserByIdAsync(id);

            return user == null ? NotFound() : Ok(user);
        }

        // GET: api/User/x@web.de
        [HttpGet("byMail/{email}")]
        public async Task<ActionResult<User>> GetUserByEmail(string email)
        {
            var user = await userService.GetUserByEmail(email);

            return user == null ? NotFound() : Ok(user);
        }

        // PUT: api/User/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<ActionResult<User>> PutUser(int id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            User? result = await userService.UpdateUserAsync(id, user);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // POST: api/User
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("register")]
        public async Task<ActionResult<User>> PostUser([FromBody] UserDto userDto)
        {
            var user = await userService.AddUserAsync(userDto);
            if (user == null)
            {
                return BadRequest("User could not be created.");
            }
            return CreatedAtAction("GetUser", new { id = user.Id }, user);
        }
        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            await userService.DeleteUserAsync(id);
            return NoContent();
        }

        [NonAction]
        public bool ValidateUser(User user, string providedPassword) => 
            userService.ValidateUser(user, providedPassword);
    }
}
