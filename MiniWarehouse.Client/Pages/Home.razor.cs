using Microsoft.AspNetCore.Components.Authorization;
using MiniWarehouse.Client.IService;

namespace MiniWarehouse.Client.Pages;

using Shared.Dto;
using Microsoft.AspNetCore.Components;
using MudBlazor;


public partial class Home
{
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private IUserApiService UserApiService { get; set; } = null!;
    [Inject] private IAuthService AuthService { get; set; } = null!;
    [Inject] private AuthenticationStateProvider AuthStateProvider { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    protected MudForm _loginForm = new();
    protected UserDto _loginModel = new();
    protected bool _isProcessing = false;
    
    protected MudForm _registerForm = new();
    protected UserDto _registerModel = new();

    protected override async Task OnInitializedAsync()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        if (authState.User.Identity?.IsAuthenticated == true)
        {
            NavigationManager.NavigateTo("/dashboard", true);
        }
    }
    
    protected async Task Register()
    {
        await _registerForm.ValidateAsync();
        if (_registerForm.IsValid)
        {
            _isProcessing = true;
            try
            {
                var success = await UserApiService.Register(_registerModel);
                if (success)
                {
                    Snackbar.Add("Registrierung erfolgreich!", Severity.Success);
                }
                else
                {
                    Snackbar.Add("Registrierung fehlgeschlagen!", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Fehler: {ex.Message}", Severity.Error);
            }
            finally
            {
                _isProcessing = false;
            }
        }
    }
    
    protected async Task Login()
    {
        await _loginForm.ValidateAsync();
        if (_loginForm.IsValid)
        {
            _isProcessing = true;
            try
            {
                var success = await AuthService.Login(_loginModel);
                if (success)
                {
                    if (AuthStateProvider is MiniWarehouse.Client.Auth.CustomAuthenticationStateProvider customProvider)
                    {
                        customProvider.NotifyUserAuthentication(_loginModel);
                    }
                    Snackbar.Add("Login erfolgreich!", Severity.Success);
                    NavigationManager.NavigateTo("/dashboard", true);
                }
                else
                {
                    Snackbar.Add("Login fehlgeschlagen!", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Fehler: {ex.Message}", Severity.Error);
            }
            finally
            {
                _isProcessing = false;
            }
        }
    }



    private bool _showLoginPassword = false;
    private bool _showRegisterPassword = false;

    private InputType LoginPasswordInputType => _showLoginPassword ? InputType.Text : InputType.Password;
    private InputType RegisterPasswordInputType => _showRegisterPassword ? InputType.Text : InputType.Password;

    private string LoginPasswordIcon => _showLoginPassword ? Icons.Material.Filled.VisibilityOff : Icons.Material.Filled.Visibility;
    private string RegisterPasswordIcon => _showRegisterPassword ? Icons.Material.Filled.VisibilityOff : Icons.Material.Filled.Visibility;
}