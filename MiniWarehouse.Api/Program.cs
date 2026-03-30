using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MiniWarehouse.Api.Data;
using MiniWarehouse.Api.Hubs;
using MiniWarehouse.Api.IService;
using MiniWarehouse.Api.Service;
using MiniWarehouse.Shared.Model;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                       ?? "Data Source=db.db";

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlite(connectionString));


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSignalR();
builder.Services.AddHostedService<WarehouseSimulator>(); 

builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient",
        policy => policy.WithOrigins("http://localhost:5164") 
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials())
                        ;
});
// Ganz oben in der Program.cs (Server), noch vor dem Builder
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
var jwtKey = builder.Configuration["Jwt:Key"] ?? "SchallplattenspielerFlurScheidungFrühlingSondereinsatzFerienUrgroßvaterAtmungKalkAbsolutismus";
var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
        ClockSkew = TimeSpan.Zero,
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
                if (context.Request.Cookies.ContainsKey("access_token"))
                {
                    context.Token = context.Request.Cookies["access_token"];
                }
                return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Auth fehlgeschlagen: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token wurde erfolgreich validiert!");
            return Task.CompletedTask;
        }
    };
});


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<DatabaseContext>();

    await db.Database.MigrateAsync();

    db.Database.Migrate();

    var userService = scope.ServiceProvider.GetRequiredService<IUserService>();
    var adminMail = "admin@MiniWarehouse.com";
    
    var adminUser = await userService.GetUserByEmail(adminMail);
    
    if (adminUser == null)
    {
        var adminDto = new MiniWarehouse.Shared.Dto.UserDto
        {
            Email = adminMail,
            Password = "admin@MiniWarehouse.com" // Das wird im Service gehasht
        };

        // 1. User anlegen
        var created = await userService.AddUserAsync(adminDto);
        
        // 2. Rolle explizit auf Admin (1) setzen
        created.Role = 1; 
        
        // 3. WICHTIG: Speichern!
        db.User.Update(created);
        await db.SaveChangesAsync();
        
        Console.WriteLine($"🚀 Admin-User '{adminMail}' wurde mit Admin-Rechten erstellt.");
    }
    var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();
    
    // 1. SQL Script ausführen (User mit Klartext-Passwort anlegen)
    var sql = await File.ReadAllTextAsync("init_users.sql");
    await db.Database.ExecuteSqlRawAsync(sql);

    // 2. Alle User fixen, die noch kein gehashtes Passwort haben
    // (EF Core Identity Hashes starten meist mit "AQAAAA...")
    var plainUsers = await db.User
        .Where(u => !u.PasswordHash.StartsWith("AQAAAA"))
        .ToListAsync();

    foreach (var user in plainUsers)
    {
        // Wir nehmen das aktuelle Klartext-Passwort und hashen es
        user.PasswordHash = hasher.HashPassword(user, user.PasswordHash);
    }

    if (plainUsers.Count != 0)
    {
        await db.SaveChangesAsync();
        Console.WriteLine($"{plainUsers.Count} User-Passwörter wurden sicher gehasht.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseCors("AllowBlazorClient");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<WarehouseHub>("/warehousehub");
app.Run();


namespace MiniWarehouse.Api
{
    public partial class Program { }
}