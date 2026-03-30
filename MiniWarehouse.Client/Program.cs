using MiniWarehouse.Client;
using MiniWarehouse.Client.Service;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using Microsoft.AspNetCore.Components.Authorization;
using MiniWarehouse.Client.Auth;
using MiniWarehouse.Client.IService;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

bool isDocker = true;

var apiBaseUrl = isDocker ? "http://api:8080/" : builder.Configuration["ApiBaseUrl"]!;

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");



builder.Services.AddScoped<CookieHandler>();
builder.Services.AddScoped(sp =>
{
    var handler = sp.GetRequiredService<CookieHandler>();
    handler.InnerHandler = new HttpClientHandler();
    return new HttpClient(handler)
    {
            BaseAddress = new Uri(builder.HostEnvironment.BaseAddress + "api/")    
    };
});

builder.Services.AddScoped<ThemeService>();
builder.Services.AddScoped<IUserApiService, UserApiService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore();

builder.Services.AddMudServices();


await builder.Build().RunAsync();
