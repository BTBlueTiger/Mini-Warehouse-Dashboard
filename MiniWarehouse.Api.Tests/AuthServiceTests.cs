using System.Threading.Tasks;
using MiniWarehouse.Api.IService;
using MiniWarehouse.Api.Service;
using MiniWarehouse.Shared.Dto;
using MiniWarehouse.Shared.Model;
using Moq;
using Xunit;

namespace MiniWarehouse.Api.Tests
{
    public class AuthServiceTests  {
            [Fact]
        public async Task GetCurrentUserAsync_ReturnsUser_WhenPrincipalAndUserExist()
        {
            // Arrange
            var user = new User { Id = 1, Email = "test@example.com" };
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(s => s.GetUserByIdAsync(1)).ReturnsAsync(user);
            var principalMock = new Mock<System.Security.Claims.ClaimsPrincipal>();
            principalMock.Setup(p => p.FindFirst("user_id")).Returns(new System.Security.Claims.Claim("user_id", "1"));
            var tokenServiceMock = new Mock<ITokenService>();
            tokenServiceMock.Setup(s => s.GetCurrentPrincipal()).ReturnsAsync(principalMock.Object);
            var authService = new AuthService(userServiceMock.Object, tokenServiceMock.Object);

            // Act
            var result = await authService.GetCurrentUserAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user, result);
        }

        [Fact]
        public async Task GetCurrentUserAsync_ReturnsNull_WhenPrincipalIsNull()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var tokenServiceMock = new Mock<ITokenService>();
            tokenServiceMock.Setup(s => s.GetCurrentPrincipal()).ReturnsAsync((System.Security.Claims.ClaimsPrincipal?)null);
            var authService = new AuthService(userServiceMock.Object, tokenServiceMock.Object);

            // Act
            var result = await authService.GetCurrentUserAsync();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetCurrentUserAsync_ReturnsNull_WhenUserIdClaimMissingOrInvalid()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var principalMock = new Mock<System.Security.Claims.ClaimsPrincipal>();
            principalMock.Setup(p => p.FindFirst("user_id")).Returns((System.Security.Claims.Claim?)null);
            var tokenServiceMock = new Mock<ITokenService>();
            tokenServiceMock.Setup(s => s.GetCurrentPrincipal()).ReturnsAsync(principalMock.Object);
            var authService = new AuthService(userServiceMock.Object, tokenServiceMock.Object);

            // Act
            var result = await authService.GetCurrentUserAsync();

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetCurrentUserAsync_ReturnsNull_WhenUserNotFound()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(s => s.GetUserByIdAsync(1)).ReturnsAsync((User?)null);
            var principalMock = new Mock<System.Security.Claims.ClaimsPrincipal>();
            principalMock.Setup(p => p.FindFirst("user_id")).Returns(new System.Security.Claims.Claim("user_id", "1"));
            var tokenServiceMock = new Mock<ITokenService>();
            tokenServiceMock.Setup(s => s.GetCurrentPrincipal()).ReturnsAsync(principalMock.Object);
            var authService = new AuthService(userServiceMock.Object, tokenServiceMock.Object);

            // Act
            var result = await authService.GetCurrentUserAsync();

            // Assert
            Assert.Null(result);
        }
    
        [Fact]
        public async Task LoginAsync_ReturnsToken_WhenCredentialsAreValid()
        {
            // Arrange
            var userDto = new UserDto { Email = "test@example.com", Password = "pw" };
            var user = new User { Id = 1, Email = userDto.Email, PasswordHash = "hashed" };
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(s => s.GetUserByEmail(userDto.Email)).ReturnsAsync(user);
            userServiceMock.Setup(s => s.ValidateUser(user, userDto.Password)).Returns(true);
            var tokenServiceMock = new Mock<ITokenService>();
            tokenServiceMock.Setup(s => s.CreateAccessToken(user)).Returns("token123");
            var authService = new AuthService(userServiceMock.Object, tokenServiceMock.Object);

            // Act
            var (success, resultUser, token) = await authService.LoginAsync(userDto);

            // Assert
            Assert.True(success);
            Assert.Equal(user, resultUser);
            Assert.Equal("token123", token);
        }

        [Fact]
        public async Task LoginAsync_ReturnsFalse_WhenUserNotFound()
        {
            var userDto = new UserDto { Email = "notfound@example.com", Password = "pw" };
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(s => s.GetUserByEmail(userDto.Email)).ReturnsAsync((User?)null);
            var tokenServiceMock = new Mock<ITokenService>();
            var authService = new AuthService(userServiceMock.Object, tokenServiceMock.Object);

            var (success, resultUser, token) = await authService.LoginAsync(userDto);

            Assert.False(success);
            Assert.Null(resultUser);
            Assert.Null(token);
        }

        [Fact]
        public async Task LoginAsync_ReturnsFalse_WhenPasswordInvalid()
        {
            var userDto = new UserDto { Email = "test@example.com", Password = "wrong" };
            var user = new User { Id = 1, Email = userDto.Email, PasswordHash = "hashed" };
            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(s => s.GetUserByEmail(userDto.Email)).ReturnsAsync(user);
            userServiceMock.Setup(s => s.ValidateUser(user, userDto.Password)).Returns(false);
            var tokenServiceMock = new Mock<ITokenService>();
            var authService = new AuthService(userServiceMock.Object, tokenServiceMock.Object);

            var (success, resultUser, token) = await authService.LoginAsync(userDto);

            Assert.False(success);
            Assert.Null(token);
        }

        [Fact]
        public async Task RefreshTokenAsync_ReturnsNewToken_WhenValidRefreshToken()
        {
            // Arrange
            var user = new User { Id = 1, Email = "test@example.com" };
            var principalMock = new Mock<System.Security.Claims.ClaimsPrincipal>();
            principalMock.Setup(p => p.FindFirst("user_id")).Returns(new System.Security.Claims.Claim("user_id", "1"));

            var userServiceMock = new Mock<IUserService>();
            userServiceMock.Setup(s => s.GetUserByIdAsync(1)).ReturnsAsync(user);
            var tokenServiceMock = new Mock<ITokenService>();
            tokenServiceMock.Setup(s => s.GetPrincipalFromExpiredToken("valid_refresh_token")).Returns(principalMock.Object);
            tokenServiceMock.Setup(s => s.CreateAccessToken(user)).Returns("new_token");

            var authService = new AuthService(userServiceMock.Object, tokenServiceMock.Object);

            // Act
            var (success, resultUser, token) = await authService.RefreshTokenAsync("valid_refresh_token");

            // Assert
            Assert.True(success);
            Assert.Equal(user, resultUser);
            Assert.Equal("new_token", token);
        }

        [Fact]
        public async Task RefreshTokenAsync_ReturnsFalse_WhenInvalidRefreshToken()
        {
            // Arrange
            var userServiceMock = new Mock<IUserService>();
            var tokenServiceMock = new Mock<ITokenService>();
            tokenServiceMock.Setup(s => s.GetPrincipalFromExpiredToken("invalid_refresh_token")).Returns((System.Security.Claims.ClaimsPrincipal?)null);

            var authService = new AuthService(userServiceMock.Object, tokenServiceMock.Object);

            // Act
            var (success, resultUser, token) = await authService.RefreshTokenAsync("invalid_refresh_token");

            // Assert
            Assert.False(success);
            Assert.Null(resultUser);
            Assert.Null(token);
        }
    
    }
}
