using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MiniWarehouse.Api.Data;
using MiniWarehouse.Api.Service;
using MiniWarehouse.Api.Tests.Helper;

using MiniWarehouse.Shared.Model;
using Moq;
using Xunit;

namespace MiniWarehouse.Api.Tests
{
    public class TokenServiceTests
    {


        [Fact]
        public void CreateToken_ReturnsValidJwt()
        {
            var context = HelperMethods.GetInMemoryDbContext();
            var httpContextAccessor = HelperMethods.GetHttpContextAccessor();
            var service = new TokenService(context, httpContextAccessor);
            var claims = new ClaimsIdentity([new Claim(ClaimTypes.Email, "test@example.com")]);
            var token = service.CreateToken(claims, DateTime.UtcNow.AddMinutes(10));

            Assert.False(string.IsNullOrWhiteSpace(token));
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            Assert.Equal(
                "test@example.com",
                jwt.Claims.First(c => c.Type == "email").Value
            );
        }

        [Fact]
        public void CreateAccessToken_ContainsUserClaims()
        {
            var context = HelperMethods.GetInMemoryDbContext();
            var httpContextAccessor = HelperMethods.GetHttpContextAccessor();
            var service = new TokenService(context, httpContextAccessor);
            var user = new User { Id = 42, Email = "foo@bar.de" };
            var token = service.CreateAccessToken(user);
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
                Assert.Equal(
                "foo@bar.de",
                jwt.Claims.First(c => c.Type == "email").Value
            );
            Assert.Equal("42", jwt.Claims.First(c => c.Type == "user_id").Value);
        }

        [Fact]
        public void CreateHttpOnlyToken_ContainsIpClaim()
        {
            var context = HelperMethods.GetInMemoryDbContext();
            var httpContextAccessor = HelperMethods.GetHttpContextAccessor("127.0.0.1");
            var service = new TokenService(context, httpContextAccessor);
            var user = new User { Id = 1, Email = "a@b.de" };
            var token = service.CreateHttpOnlyToken(user);
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            Assert.Equal("a@b.de", jwt.Claims.First(c => c.Type == "email").Value);
            Assert.Equal("1", jwt.Claims.First(c => c.Type == "user_id").Value);
            Assert.Equal("127.0.0.1", jwt.Claims.First(c => c.Type == "user_ip").Value);
        }

        [Fact]
        public void ValidateToken_ReturnsPrincipalForValidToken()
        {
            var context = HelperMethods.GetInMemoryDbContext();
            var httpContextAccessor = HelperMethods.GetHttpContextAccessor();
            var service = new TokenService(context, httpContextAccessor);
            var user = new User { Id = 1, Email = "a@b.de" };
            var token = service.CreateAccessToken(user);
            var principal = service.ValidateToken(token);
            Assert.NotNull(principal);
            Assert.Equal("a@b.de", principal.FindFirst(ClaimTypes.Email)?.Value);
        }

        [Fact]
        public async Task DeleteTokenAsync_RemovesToken()
        {
            var context = HelperMethods.GetInMemoryDbContext();
            var httpContextAccessor = HelperMethods.GetHttpContextAccessor();
            var service = new TokenService(context, httpContextAccessor);
            var tokenEvent = new UserTokenEvent { Id = 99, Token = "abc", Type = 0, UserId = 1, CreatedAt = DateTime.UtcNow, ExpiresAt = DateTime.UtcNow.AddHours(1) };
            context.UserTokenEvent.Add(tokenEvent);
            context.SaveChanges();
            await service.DeleteTokenAsync(99);
            Assert.Null(await context.UserTokenEvent.FindAsync(99));
        }

        [Fact]
        public void ValidateToken_ThrowsOnInvalidToken()
        {
            var context = HelperMethods.GetInMemoryDbContext();
            var httpContextAccessor = HelperMethods.GetHttpContextAccessor();
            var service = new TokenService(context, httpContextAccessor);
            Assert.Throws<UnauthorizedAccessException>(() => service.ValidateToken("invalid.token.value"));
        }
    }
}
