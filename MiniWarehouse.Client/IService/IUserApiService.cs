using MiniWarehouse.Shared.Dto;
using MiniWarehouse.Shared.Model;

namespace MiniWarehouse.Client.IService;

public interface IUserApiService
{
        Task<bool> Register(UserDto user);
        Task<bool> DeleteAccount(string userId);
        Task<User> GetUserById(string id);
        Task<User> GetUserByEmail(string email);
        Task<User> UpdateUser(User user);
        Task<bool> DeleteUser(User user);
        Task<List<User>> GetAllUsers();
}
