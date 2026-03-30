using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MiniWarehouse.Api.Data;
using Moq;

namespace MiniWarehouse.Api.Tests.Helper
{
    public static class HelperMethods
    {
        public static DatabaseContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;
            return new DatabaseContext(options);
        }
        public static IHttpContextAccessor GetHttpContextAccessor(string? ip = null)
        {
            var mock = new Mock<IHttpContextAccessor>();
            var context = new DefaultHttpContext();
            if (ip != null)
            {
                context.Connection.RemoteIpAddress = System.Net.IPAddress.Parse(ip);
            }
            mock.Setup(a => a.HttpContext).Returns(context);
            return mock.Object;
        }
    }
}