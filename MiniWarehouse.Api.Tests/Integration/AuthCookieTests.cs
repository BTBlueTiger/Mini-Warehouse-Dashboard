using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MiniWarehouse.Api.Data;
using Xunit;

namespace MiniWarehouse.Api.Tests.Integration
{
    public class AuthCookieTests(WebApplicationFactory<MiniWarehouse.Api.Program> factory) : IClassFixture<WebApplicationFactory<MiniWarehouse.Api.Program>>
    {
        private readonly WebApplicationFactory<MiniWarehouse.Api.Program> _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // alte Registrierung entfernen
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<DatabaseContext>));

                    if (descriptor != null)
                        services.Remove(descriptor);

                    services.AddDbContext<DatabaseContext>(options =>
                        options.UseSqlite("Data Source=test.db"));
                });
            });

        [Fact]
        public async Task Login_Sets_HttpOnly_AuthCookie()
        {
            var client = _factory.CreateClient();

            var registerDto = new { Email = "test@example.com", Password = "123456" };
            var response = await client.PostAsJsonAsync("/api/User/register", registerDto);
            var content = await response.Content.ReadAsStringAsync();
            System.Console.WriteLine(content);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                
            
            var loginDto = new { Email = "test@example.com", Password = "123456" };
            response = await client.PostAsJsonAsync("/api/Auth/login", loginDto);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var setCookie = response.Headers.TryGetValues("Set-Cookie", out var values) ? values.FirstOrDefault() : null;
            Assert.NotNull(setCookie);
            Assert.Contains("httponly", setCookie.ToLower());            // Optional: Prüfe auf den Cookie-Namen, z.B. "auth_token"
            // Assert.Contains("auth_token", setCookie);
        }
    }
}
