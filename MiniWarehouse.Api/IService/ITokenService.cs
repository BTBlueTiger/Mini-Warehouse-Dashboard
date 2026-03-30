using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using MiniWarehouse.Shared.Model;

namespace MiniWarehouse.Api.IService
{
    public interface ITokenService
    {
        string CreateToken(ClaimsIdentity claims, DateTime expired_at);
        string CreateAccessToken(User user);
        string CreateHttpOnlyToken(User user);
        Task DeleteTokenAsync(int tokenId);
        ClaimsPrincipal ValidateToken(string token);
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);

        Task<ClaimsPrincipal?> GetCurrentPrincipal();
    }
}