using System.Net;
using System.Text;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using Utya.Services;

namespace Utya.Tests;

public class GeoLocatorTests
{
    [Fact]
    public async Task GetCountryCode_ShouldReturnRU_ForLocalhost()
    {
        // Arrange
        var contextMock = new Mock<HttpContext>();
        contextMock.Setup(x => x.Connection.RemoteIpAddress)
            .Returns(IPAddress.Loopback);

        var clientHandler = new Mock<HttpMessageHandler>();
        clientHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                Content = new StringContent("{'countryCode': 'RU'}", Encoding.UTF8, "application/json")
            });

        var httpClient = new HttpClient(clientHandler.Object);
        var service = new GeoIpService(httpClient, Mock.Of<ILogger<GeoIpService>>());

        // Act
        var result = await service.GetCountryCode(contextMock.Object.Connection.RemoteIpAddress?.ToString());

        // Assert
        result.Should().Be("RU");
    }

    [Fact]
    public async Task GetCountryCode_ShouldHandleInvalidResponse()
    {
        // Arrange
        var contextMock = new Mock<HttpContext>();
        contextMock.Setup(x => x.Connection.RemoteIpAddress)
            .Returns(IPAddress.Parse("8.8.8.8"));

        var clientHandler = new Mock<HttpMessageHandler>();
        clientHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), CancellationToken.None)
            .ThrowsAsync(new HttpRequestException("Timeout"));

        var httpClient = new HttpClient(clientHandler.Object);
        var loggerMock = new Mock<ILogger<GeoIpService>>();
        var service = new GeoIpService(httpClient, loggerMock.Object);

        // Act
        var result = await service.GetCountryCode(contextMock.Object.Connection.RemoteIpAddress?.ToString());

        // Assert
        result.Should().BeNull();
    }
}