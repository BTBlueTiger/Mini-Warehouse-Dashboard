
using Microsoft.AspNetCore.Mvc;
using MiniWarehouse.Shared.Dto;
using MiniWarehouse.Shared.Model;

namespace MiniWarehouse.Api.IService;

public interface IUserService
{
    Task<IEnumerable<User>> GetUsersAsync();
    Task<User?> GetUserByIdAsync(int id);
    Task<User?> GetUserByEmail(string email);
    Task<User> AddUserAsync(UserDto userDto);
    Task<User?> UpdateUserAsync(int id, User user);
    Task UpdateLastInteractionAsync(int userId);
    Task DeleteUserAsync(int id);
    bool ValidateUser(User user, string password);
}
    