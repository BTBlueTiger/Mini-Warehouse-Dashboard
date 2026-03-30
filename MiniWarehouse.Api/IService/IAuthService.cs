using System.Threading.Tasks;
using MiniWarehouse.Shared.Dto;
using MiniWarehouse.Shared.Model;

namespace MiniWarehouse.Api.IService
{
    public interface IAuthService
    {
        Task<(bool Success, User? User, string? Token)> LoginAsync(UserDto userDto);
        Task<(bool Success, User? User, string? Token)> RefreshTokenAsync(string refreshToken);
        Task<User?> GetCurrentUserAsync();
    }
}
