using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MiniWarehouse.Api.Data;
using MiniWarehouse.Api.Service;
using MiniWarehouse.Shared.Dto;
using MiniWarehouse.Shared.Model;
using MiniWarehouse.Api.Tests.Helper;
using Xunit;

namespace MiniWarehouse.Api.Tests
{
    public class UserServiceTests
    {

        [Fact]
        public async Task AddUserAsync_CreatesUserWithHashedPassword()
        {
            // Arrange
            var context = HelperMethods.GetInMemoryDbContext();
            var passwordHasher = new PasswordHasher<User>();
            var service = new UserService(context, passwordHasher);
            var userDto = new UserDto { Email = "test@example.com", Password = "secret123" };

            // Act
            var user = await service.AddUserAsync(userDto);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(userDto.Email, user.Email);
            Assert.NotNull(user.PasswordHash);
            Assert.NotEqual(userDto.Password, user.PasswordHash);
            Assert.Single(context.User);
        }

        [Fact]
        public async Task GetUserByIdAsync_ReturnsUser()
        {
            var context = HelperMethods.GetInMemoryDbContext();
            var passwordHasher = new PasswordHasher<User>();
            var service = new UserService(context, passwordHasher);
            var user = new User { Email = "a@b.de", PasswordHash = "pw" };
            context.User.Add(user);
            context.SaveChanges();

            var result = await service.GetUserByIdAsync(user.Id);
            Assert.NotNull(result);
            Assert.Equal(user.Email, result.Email);
        }

        [Fact]
        public async Task GetUsersAsync_ReturnsAllUsers()
        {
            var context = HelperMethods.GetInMemoryDbContext();
            var passwordHasher = new PasswordHasher<User>();
            var service = new UserService(context, passwordHasher);
            context.User.Add(new User { Email = "a@b.de", PasswordHash = "pw" });
            context.User.Add(new User { Email = "b@b.de", PasswordHash = "pw2" });
            context.SaveChanges();

            var users = await service.GetUsersAsync();
            Assert.Equal(2, users.Count());
        }

        [Fact]
        public async Task UpdateUserAsync_UpdatesFields()
        {
            var context = HelperMethods.GetInMemoryDbContext();
            var passwordHasher = new PasswordHasher<User>();
            var service = new UserService(context, passwordHasher);
            var user = new User { Email = "a@b.de", Name = "Old", PasswordHash = "pw" };
            context.User.Add(user);
            context.SaveChanges();

            user.Name = "New";
            var updated = await service.UpdateUserAsync(user.Id, user);
            Assert.NotNull(updated);
            Assert.Equal("New", updated.Name);
        }

        [Fact]
        public async Task DeleteUserAsync_RemovesUser()
        {
            var context = HelperMethods.GetInMemoryDbContext();
            var passwordHasher = new PasswordHasher<User>();
            var service = new UserService(context, passwordHasher);
            var user = new User { Email = "a@b.de", PasswordHash = "pw" };
            context.User.Add(user);
            context.SaveChanges();

            await service.DeleteUserAsync(user.Id);
            Assert.Empty(context.User);
        }

        [Fact]
        public void ValidateUser_ReturnsTrueForCorrectPassword()
        {
            var passwordHasher = new PasswordHasher<User>();
            var user = new User { Email = "a@b.de" };
            var password = "secret123";
            user.PasswordHash = passwordHasher.HashPassword(user, password);
            var context = HelperMethods.GetInMemoryDbContext();
            var service = new UserService(context, passwordHasher);

            var result = service.ValidateUser(user, password);
            Assert.True(result);
        }

        [Fact]
        public void ValidateUser_ReturnsFalseForWrongPassword()
        {
            var passwordHasher = new PasswordHasher<User>();
            var user = new User { Email = "a@b.de" };
            user.PasswordHash = passwordHasher.HashPassword(user, "secret123");
            var context = HelperMethods.GetInMemoryDbContext();
            var service = new UserService(context, passwordHasher);

            var result = service.ValidateUser(user, "wrong");
            Assert.False(result);
        }
    }
}
