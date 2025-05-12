using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Utya.Data;
using Utya.Models;
using Utya.Services;

namespace Utya.Tests;

public class ShortLinkServiceTests
{
    private readonly ShortLinkService _service;

    public ShortLinkServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var dbContext = new ApplicationDbContext(options);
        var passwordHasher = new PasswordHasher<ShortLink>();
        var geoLocatorMock = new Mock<IGeoLocator>();

        _service = new ShortLinkService(
            dbContext,
            passwordHasher,
            geoLocatorMock.Object,
            Mock.Of<ILogger<ShortLinkService>>()
        );
    }

    [Fact]
    public async Task CreateShortLink_ShouldGenerateUniqueCode()
    {
        // Act
        var link1 = await _service.CreateShortLinkAsync(new CreateShortLinkRequest("https://test1.com"));
        var link2 = await _service.CreateShortLinkAsync(new CreateShortLinkRequest("https://test2.com"));

        // Assert
        link1.ShortCode.Should().NotBe(link2.ShortCode);
        link1.ShortCode.Should().HaveLength(6);
    }

    [Fact]
    public async Task CreateShortLink_ShouldValidateCustomAlias()
    {
        // Arrange
        var request = new CreateShortLinkRequest("https://test.com", "my-alias");

        // Act
        await _service.CreateShortLinkAsync(request);
        var act = () => _service.CreateShortLinkAsync(request);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Этот алиас уже занят");
    }

    [Fact]
    public async Task CreateShortLink_ShouldHashPassword()
    {
        // Arrange
        var request = new CreateShortLinkRequest("https://test.com")
        {
            Password = "secret123"
        };

        // Act
        var link = await _service.CreateShortLinkAsync(request);

        // Assert
        link.PasswordHash.Should().NotBeNullOrEmpty()
            .And.NotBe("secret123");
        // _service.VerifyPassword(link, "secret123").Should().BeTrue();
    }

    [Fact]
    public async Task CreateShortLink_ShouldExpireLinks()
    {
        // Arrange
        var request = new CreateShortLinkRequest("https://test.com")
        {
            ExpiresAt = DateTime.UtcNow.AddDays(-1)
        };

        // Act
        var link = await _service.CreateShortLinkAsync(request);

        // Assert
        link.IsExpired.Should().BeTrue();
    }
}