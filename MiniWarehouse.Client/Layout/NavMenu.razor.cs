using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using MiniWarehouse.Client.IService;

namespace MiniWarehouse.Client.Layout;

public partial class NavMenu
{
    [Inject] private IAuthService AuthService { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = null!;
    [Inject] private NavigationManager navigationManager { get; set; } = null!;
    
    protected async Task Logout()
    {
        try
        {
            await AuthService.Logout();
            if (AuthStateProvider is Auth.CustomAuthenticationStateProvider customProvider)
            {
                customProvider.NotifyUserLogout();
            }
            navigationManager.NavigateTo("/");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[NavMenu] Logout-Fehler: {ex.Message}");
        }
    }
    protected override void OnInitialized()
    {
        if (AuthStateProvider is Auth.CustomAuthenticationStateProvider customProvider)
        {
            customProvider.AuthenticationStateChanged += async (task) =>
            {
                var authState = await task;
                if (!authState.User.Identity?.IsAuthenticated == true)
                {
                    navigationManager.NavigateTo("/", true);
                }
            };
        }
    }
    
}