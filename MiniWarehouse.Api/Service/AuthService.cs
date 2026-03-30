using System.Threading.Tasks;
using MiniWarehouse.Api.IService;
using MiniWarehouse.Shared.Dto;
using MiniWarehouse.Shared.Model;

namespace MiniWarehouse.Api.Service
{
    public class AuthService(IUserService userService, ITokenService tokenService) : IAuthService
    {
        public async Task<(bool Success, User? User, string? Token)> LoginAsync(UserDto userDto)
        {
            var user = await userService.GetUserByEmail(userDto.Email);
            if (user == null || !userService.ValidateUser(user, userDto.Password))
            {
                return (false, null, null);
            }

            var token = tokenService.CreateAccessToken(user);
            return (true, user, token);
        }

        public Task<(bool Success, User? User, string? Token)> RefreshTokenAsync(string refreshToken)
        {
            var principal = tokenService.GetPrincipalFromExpiredToken(refreshToken);
            if (principal == null)
                return Task.FromResult((false, (User?)null, (string?)null));

            var userId = principal.FindFirst("user_id")?.Value;
            if (string.IsNullOrEmpty(userId))
                return Task.FromResult((false, (User?)null, (string?)null));

            return RefreshTokenInternalAsync(userService, tokenService, int.Parse(userId));
        }

        private async Task<(bool Success, User? User, string? Token)> RefreshTokenInternalAsync(
            IUserService userService,
            ITokenService tokenService,
            int userId)
        {
            var user = await userService.GetUserByIdAsync(userId);
            if (user == null)
                return (false, null, null);

            var token = tokenService.CreateAccessToken(user);
            return (true, user, token);
        }

        public async Task<User?> GetCurrentUserAsync()
        {
            // Hole ClaimsPrincipal aus dem aktuellen Kontext
            var userPrincipal = await tokenService.GetCurrentPrincipal();
            var userIdClaim = userPrincipal?.FindFirst("user_id");
            if (userPrincipal == null) return null;

            // Debug-Ausgabe: Was steckt wirklich drin?
            foreach (var claim in userPrincipal.Claims)
            {
                Console.WriteLine($"Claim Typ: {claim.Type}, Wert: {claim.Value}");
            }

            // Lade den User aus der Datenbank
            return await userService.GetUserByIdAsync(userIdClaim != null ? int.Parse(userIdClaim.Value) : 0);
        }
    }
}
