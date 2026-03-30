using System.Net.Http.Json;
using MiniWarehouse.Client.IService;
using MiniWarehouse.Shared.Dto;

namespace MiniWarehouse.Client.Service;

public class AuthService(HttpClient http) : IAuthService
{
    public async Task<bool> Login(UserDto user) 
        => (await http.PostAsync("api/auth/login", JsonContent.Create(user))).IsSuccessStatusCode;

    public async Task<bool> Logout()
        => (await http.PostAsync("api/auth/logout", null)).IsSuccessStatusCode;

    public async Task<UserDto> AuthMe() 
        => await http.GetFromJsonAsync<UserDto>("api/auth/me") ?? new UserDto();
}