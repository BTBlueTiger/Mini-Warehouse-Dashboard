
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using MiniWarehouse.Client.Service;
using Moq;
using Moq.Protected;

namespace MiniWarehouse.Client.Tests.Helpers
{
public static class TestHelper
{
    public static WareHouseApiService CreateMockedService(HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = statusCode
            });

        var httpClient = new HttpClient(handlerMock.Object)
        {
            BaseAddress = new Uri("http://localhost:5020/")
        };
        return new WareHouseApiService(httpClient);
    }
}
}