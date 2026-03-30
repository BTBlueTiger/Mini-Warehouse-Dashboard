using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using MiniWarehouse.Shared.Dto;
using Moq;
using Moq.Protected;
using Xunit;
using MiniWarehouse.Client.Service;
using System.Net;
using MiniWarehouse.Client.Tests.Helpers; // Für HttpStatusCode

namespace MiniWarehouse.Client.Tests.Service
{
    public class WareHouseApiServiceTests
    {
        [Fact]
        public async Task Login_ReturnsTrue_OnSuccess()
        {
            var service = TestHelper.CreateMockedService();
            var result = await service.Login(new UserDto { Email = "test@web.de", Password = "123456" });
            Assert.True(result);
        }

        [Fact]
        public async Task Login_ReturnsFalse_OnFailure()
        {
            var service = TestHelper.CreateMockedService(HttpStatusCode.Unauthorized);
            var result = await service.Login(new UserDto { Email = "test@web.de", Password = "223456" });
            Assert.False(result);
        }

        [Fact]
        public async Task Register_ReturnsTrue_OnSuccess()
        {
            var service = TestHelper.CreateMockedService(HttpStatusCode.OK);
            var result = await service.Register(new UserDto { Email = "test@web.de", Password = "123456" });
            Assert.True(result);
        }

        [Fact]
        public async Task Register_ReturnsFalse_OnFailure()
        {
            var service = TestHelper.CreateMockedService(HttpStatusCode.Unauthorized);
            var result = await service.Register(new UserDto { Email = "test@web.de", Password = "223456" });
            Assert.False(result);
        }

        [Fact]
        public async Task Logout_ThrowsNotImplementedException()
        {
            var service = TestHelper.CreateMockedService();
            await Assert.ThrowsAsync<NotImplementedException>(() => service.Logout());
        }

        [Fact]
        public async Task IsAuthenticated_ThrowsNotImplementedException()
        {
            var service = TestHelper.CreateMockedService();
            await Assert.ThrowsAsync<NotImplementedException>(() => service.IsAuthenticated());
        }

        [Fact]
        public async Task GetToken_ThrowsNotImplementedException()
        {
            var service = TestHelper.CreateMockedService();
            await Assert.ThrowsAsync<NotImplementedException>(() => service.GetToken());
        }

        [Fact]
        public async Task DeleteAccount_ThrowsNotImplementedException()
        {
            var service = TestHelper.CreateMockedService();
            await Assert.ThrowsAsync<NotImplementedException>(() => service.DeleteAccount("1"));
        }

        [Fact]
        public async Task GetUserById_ThrowsNotImplementedException()
        {
            var service = TestHelper.CreateMockedService();
            await Assert.ThrowsAsync<NotImplementedException>(() => service.GetUserById("1"));
        }

        [Fact]
        public async Task GetUserByEmail_ThrowsNotImplementedException()
        {
            var service = TestHelper.CreateMockedService();
            await Assert.ThrowsAsync<NotImplementedException>(() => service.GetUserByEmail("test@web.de"));
        }
    }

}