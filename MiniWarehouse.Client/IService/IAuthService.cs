using MiniWarehouse.Shared.Dto;
using MiniWarehouse.Shared.Model;

namespace MiniWarehouse.Client.IService;

public interface IAuthService
{
    Task<bool> Login(UserDto user);
    Task<bool> Logout();
    Task<UserDto> AuthMe();
}