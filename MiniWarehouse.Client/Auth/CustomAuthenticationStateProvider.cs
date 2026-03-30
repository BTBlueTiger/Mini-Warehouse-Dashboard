using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using MiniWarehouse.Shared.Dto;
using MiniWarehouse.Client.IService;

namespace MiniWarehouse.Client.Auth;

public class CustomAuthenticationStateProvider(IAuthService authService, IUserApiService userApiService) : AuthenticationStateProvider
{
    private ClaimsPrincipal _anonymous = new(new ClaimsIdentity());

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var userDto = await authService.AuthMe();
            if (string.IsNullOrEmpty(userDto.Email))
                return new AuthenticationState(_anonymous);

            var identity = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.Name, userDto.Email ?? ""),
                new Claim(ClaimTypes.Role, userDto.Role == 1 ? "Admin" : "User"),
            ], "serverAuth");
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AuthProvider] Fehler: {ex.Message}");
            return new AuthenticationState(_anonymous);
        }
    }

    public void NotifyUserAuthentication(UserDto userDto)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, userDto.Email ?? ""),
            new Claim(ClaimTypes.Role, userDto.Role == 1 ? "Admin" : "User")
        };

        // WICHTIG: Der 3. und 4. Parameter sagen Blazor: 
        // "Das hier ist der Name-Claim und DAS hier ist der Rollen-Claim!"
        var identity = new ClaimsIdentity(claims, "serverAuth", ClaimTypes.Name, ClaimTypes.Role);
    
        var user = new ClaimsPrincipal(identity);
        var state = Task.FromResult(new AuthenticationState(user));
    
        NotifyAuthenticationStateChanged(state);
    }
    public void NotifyUserLogout()
    {
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }
}

