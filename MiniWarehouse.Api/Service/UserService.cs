using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiniWarehouse.Api.Data;
using MiniWarehouse.Api.IService;
using MiniWarehouse.Shared.Dto;
using MiniWarehouse.Shared.Model;

namespace MiniWarehouse.Api.Service
{
    public class UserService(DatabaseContext context, IPasswordHasher<User> passwordHasher) : IUserService
    {
        private readonly DatabaseContext _context = context;
        private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;

        private bool UserExists(int id) =>
            _context.User.Any(e => e.Id == id);

        public async Task<User> AddUserAsync(UserDto userDto)
        {
            var user = new User
            {
                Email = userDto.Email,
                PasswordHash = _passwordHasher.HashPassword(new User { Email = userDto.Email }, userDto.Password), // Hash the password before saving
                LastInteraction = DateTime.UtcNow
            };

            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return;
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetUserByEmail(string email) =>
            await _context.User.FirstOrDefaultAsync(u => u.Email == email);


        public async Task<User?> GetUserByIdAsync(int id) =>
            await _context.User.FindAsync(id);

        public Task<IEnumerable<User>> GetUsersAsync() => 
            Task.FromResult(_context.User.AsEnumerable());

        public async Task<User?> UpdateUserAsync(int id, User user)
        {
            var existingUser = await GetUserByIdAsync(id);
            if (existingUser == null)
            {
                return null;
            }

            // Update only allowed properties
            existingUser.Email = user.Email;
            existingUser.Name = user.Name;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.Role = user.Role;
            existingUser.LastInteraction = DateTime.UtcNow;
            // PasswordHash should only be updated if explicitly changed (not shown here)

            _context.Entry(existingUser).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }

            return existingUser;
        }

        public bool ValidateUser(User user, string password)
        {
            if (user.PasswordHash == null)
            {
                return false;
            }
            // Verify the password
            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                password
            );
            return result == PasswordVerificationResult.Success;
        }

        public async Task UpdateLastInteractionAsync(int userId)
        {
            var user = await _context.User.FindAsync(userId);
            if (user != null)
            {
                user.LastInteraction = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }
    }
    
}