using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniWarehouse.Api.Controllers;
using MiniWarehouse.Api.Data;
using MiniWarehouse.Api.Model;
using Xunit;

namespace MiniWarehouse.Api.Tests
{
    public class UserControllerTests
    {
        private DatabaseContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            return new DatabaseContext(options);
        }

        [Fact]
        public async Task GetUser_ReturnsAllUsers()
        {
            // Arrange
            var context = GetInMemoryDbContext();
            _ = context.User.Add(new User { Name = "Test", Email = "test@example.com", PasswordHash = "pw" });
            _ = context.User.Add(new User { Name = "Test2", Email = "test2@example.com", PasswordHash = "pw2" });
            _ = context.SaveChanges();
            var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
            var controller = new UserController(context, passwordHasher);

            // Act
            var result = await controller.GetUser();

            // Assert
            Assert.NotNull(result.Value);
            Assert.Equal(2, ((List<User>)result.Value).Count);
        }

        [Fact]
        public async Task GetUser_ById_ReturnsUser()
        {
            var context = GetInMemoryDbContext();
                
            var user = new User { Name = "Test", Email = "test@example.com", PasswordHash = "pw" };
            _ = context.User.Add(user);
            _ = context.SaveChanges();
            var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();  
            var controller = new UserController(context, passwordHasher);

            var result = await controller.GetUser(user.Id);

            Assert.NotNull(result.Value);
            Assert.Equal(user.Email, result.Value.Email);
        }

        [Fact]
        public async Task PostUser_CreatesUser()
        {
            var context = GetInMemoryDbContext();
            var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
            var controller = new UserController(context, passwordHasher);
            var user = new Model.Dto.UserRegisterDto { Email = "new@example.com", Password = "pw" };

            var actionResult = await controller.PostUser(user);
            var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            var createdUser = Assert.IsType<User>(createdResult.Value);
            Assert.Equal("new@example.com", createdUser.Email);
            _ = Assert.Single(context.User);
        }

        [Fact]
        public async Task PutUser_UpdatesUser()
        {
            var context = GetInMemoryDbContext();
            var user = new User { Name = "Old", Email = "old@example.com", PasswordHash = "pw" };
            _ = context.User.Add(user);
            _ = context.SaveChanges();
            var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
            var controller = new UserController(context, passwordHasher);

            user.Name = "Updated";
            var result = await controller.PutUser(user.Id, user);

            _ = Assert.IsType<NoContentResult>(result);
            Assert.Equal("Updated", context.User.Find(user.Id).Name);
        }

        [Fact]
        public async Task DeleteUser_RemovesUser()
        {
            var context = GetInMemoryDbContext();
            var user = new User { Name = "ToDelete", Email = "del@example.com", PasswordHash = "pw" };
            _ = context.User.Add(user);
            _ = context.SaveChanges();
            var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
            var controller = new UserController(context, passwordHasher);

            var result = await controller.DeleteUser(user.Id);

            _ = Assert.IsType<NoContentResult>(result);
            Assert.Empty(context.User);
        }

        [Fact]
        public void PasswordHasher_Hashes_And_Verifies_Correctly()
        {
            var user = new User { Email = "test@example.com" };
            var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<User>();
            var password = "meinPasswort123";
            var hash = hasher.HashPassword(user, password);

            // Hash sollte nicht gleich dem Klartext sein
            Assert.NotEqual(password, hash);

            // Überprüfung muss erfolgreich sein
            var result = hasher.VerifyHashedPassword(user, hash, password);
            Assert.Equal(Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success, result);
        }
    }
}
