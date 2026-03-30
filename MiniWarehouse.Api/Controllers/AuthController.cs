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
    public class AuthController(IAuthService authService, IUserService userService) : ControllerBase
    {
        // POST: api/Auth/login
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login([FromBody] UserDto userDto)
        {
            var (success, user, token) = await authService.LoginAsync(userDto);            

            if(!success ||token == null || user == null)
            {
                return Unauthorized();
            }

            Response.Cookies.Append(
                "access_token",
                token,
                new CookieOptions { 
                    HttpOnly = true,
                    Path = "/",
                    Secure = false, // Wichtig für lokales HTTP ohne SSL
                    SameSite = SameSiteMode.Lax,
                    }
            );

            await userService.UpdateLastInteractionAsync(user.Id);

            return Ok(user);
        }

        [HttpPost("logout")]
        public async Task<ActionResult> Logout()
        {

            var user = await authService.GetCurrentUserAsync();
            if(user != null)
            {
                await userService.UpdateLastInteractionAsync(user.Id);
            }
            
            Response.Cookies.Delete("access_token", new CookieOptions {
                Path = "/",
                HttpOnly = true,
                Secure = false, // wie beim Setzen
                SameSite = SameSiteMode.Lax
            });

            return Ok();
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> Me()
        {
            var user = await authService.GetCurrentUserAsync();
            Console.WriteLine($"Current user: {user?.Email}");
            
            if (user == null)
            {
                return Unauthorized();
            }
            await userService.UpdateLastInteractionAsync(user.Id);

            return Ok(user);
        }
    }
}