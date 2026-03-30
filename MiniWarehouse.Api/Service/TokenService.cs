using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MiniWarehouse.Api.Data;
using MiniWarehouse.Api.IService;
using MiniWarehouse.Shared.Dto;
using MiniWarehouse.Shared.Model;
using NuGet.Protocol.Plugins;

namespace MiniWarehouse.Api.Service
{
    public class TokenService(DatabaseContext context, IHttpContextAccessor httpContextAccessor) : ITokenService
    {
        private readonly JwtSecurityTokenHandler _tokenHandler = new();

        /**
            * Note: In a production application, the secret key should be stored securely, such as in environment variables or a secure vault.
            * The key should also be of sufficient length and complexity to prevent brute-force attacks.
        */
        
        // Never doing this in production! The secret key should be stored securely and not hardcoded in the source code.
        static readonly string superSecretKey = "SchallplattenspielerFlurScheidungFrühlingSondereinsatzFerienUrgroßvaterAtmungKalkAbsolutismus"; // Must be at least 32 characters for HmacSha256

        
        static private readonly byte[] _key = System.Text.Encoding.UTF8.GetBytes(superSecretKey);
        private readonly SigningCredentials _credentials = 
            new(
                new SymmetricSecurityKey(_key)
            , SecurityAlgorithms.HmacSha256);
        private readonly int _accessTokenValidDuration = 60; // Duration in minutes

    
        public string CreateToken(ClaimsIdentity claims, DateTime expired_at)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = expired_at,
                SigningCredentials = _credentials,
            };
            var token = _tokenHandler.CreateToken(tokenDescriptor);
            Console.WriteLine($"Token ValidTo: {token.ValidTo}"); // Check expiration
            return _tokenHandler.WriteToken(token);
        }

        public string CreateAccessToken(User user)
        {
            var claims = new List<Claim>
            {
                new("email", user.Email),
                new("user_id", user.Id.ToString()),
            };
            var identity = new ClaimsIdentity(claims);
            var token = CreateToken(identity, DateTime.UtcNow.AddMinutes(_accessTokenValidDuration));
            return token;   
        }

        public string CreateHttpOnlyToken(User user)
        {
            var httpContext = httpContextAccessor.HttpContext;
            string? userIp = null;
            if (httpContext != null && httpContext.Connection.RemoteIpAddress != null)
            {
                userIp = httpContext.Connection.RemoteIpAddress.ToString();
                // Do something with userIp
            }

            var claims = new List<Claim>
            {
                new("email", user.Email),
                new("user_id", user.Id.ToString()),
                new("user_ip", userIp ?? "No IP Found"),
            };

            var identity = new ClaimsIdentity(claims);
            var token = CreateToken(identity, DateTime.UtcNow.AddHours(4));
            return token;
        }

        public Task<IActionResult> CheckHttpOnlyToken()
        {
            throw new NotImplementedException();
        }

        public async Task DeleteTokenAsync(int tokenId)
        {
            var token = await context.UserTokenEvent.FindAsync(tokenId);
            if (token != null)
            {
                context.UserTokenEvent.Remove(token);
                await context.SaveChangesAsync();
            }
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(_key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.FromMinutes(5), // Allows a 5-minute drift
                ValidateLifetime =
                    true // Ensures token expiration is validated
                ,
            };

            try
            {
                // Validate the token and get the principal
                var principal = _tokenHandler.ValidateToken(
                    token,
                    validationParameters,
                    out SecurityToken validatedToken
                );
                return principal;
            }
            catch (Exception ex)
            {
                // Always throw UnauthorizedAccessException for any error
                throw new UnauthorizedAccessException("Invalid token.", ex);
            }
        }

        public ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var validationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(_key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero, // No clock skew for expired tokens
                ValidateLifetime = false // Ignore token expiration
            };

            try
            {
                var principal = _tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
                if (validatedToken is JwtSecurityToken jwtToken &&
                    jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return principal;
                }
            }
            catch
            {
                // Return null for any validation failure
            }

            return null;
        }

        public Task<ClaimsPrincipal?> GetCurrentPrincipal()
        {
            var httpContext = httpContextAccessor.HttpContext;
            Console.WriteLine($"HttpContext is null: {httpContext == null}");
            Console.WriteLine($"IsAuthenticated: {httpContext.User.Identity?.IsAuthenticated}");
            if (httpContext == null)
                return Task.FromResult<ClaimsPrincipal?>(null);

            var userPrincipal = httpContext.User;

        // Debug-Ausgabe: Was steckt wirklich drin?
            foreach (var claim in httpContext.User.Claims)
            {
                Console.WriteLine($"Claim Typ: {claim.Type}, Wert: {claim.Value}");
            }
            if (userPrincipal != null)
                return Task.FromResult<ClaimsPrincipal?>(userPrincipal);

            return Task.FromResult<ClaimsPrincipal?>(null);
        }
    }
}