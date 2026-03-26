
using MiniWarehouse.Api.Model;

namespace MiniWarehouse.Api.IService;

public interface IUserService
{
    Task<IEnumerable<User>> GetUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task<User?> GetUserByEmailOrPhoneAsync(string email, string phoneNumber);
    Task<User?> GetUserByEmail(string email);
    Task<User> AddUserAsync(User user);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(int id);
    string HashPassword(User user, string password);
    bool ValidateUser(User user, string password);
}
