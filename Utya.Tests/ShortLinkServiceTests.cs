using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Utya.Data;
using Utya.Services;
using Utya.Shared.Models;

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

        var userManagerMock = new Mock<UserManager<ApplicationUser>>(
            Mock.Of<IUserStore<ApplicationUser>>(),
            null, null, null, null, null, null, null, null);
            
        var limitServiceMock = new Mock<ILimitService>();
        limitServiceMock.Setup(x => x.CanCreateLinkAsync(It.IsAny<string>()))
            .ReturnsAsync(true);
            
        _service = new ShortLinkService(
            dbContext,
            passwordHasher,
            geoLocatorMock.Object,
            Mock.Of<ILogger<ShortLinkService>>(),
            userManagerMock.Object,
            limitServiceMock.Object
        );
    }

    [Fact]
    public async Task CreateShortLink_ShouldGenerateUniqueCode()
    {
        // Arrange
        var user = new ApplicationUser { Id = Guid.NewGuid().ToString(), UserName = "testuser" };
        
        // Act
        var link1 = await _service.CreateShortLinkAsync(new CreateShortLinkRequest("https://test1.com"), user);
        var link2 = await _service.CreateShortLinkAsync(new CreateShortLinkRequest("https://test2.com"), user);

        // Assert
        link1.ShortCode.Should().NotBe(link2.ShortCode);
        link1.ShortCode.Should().HaveLength(6);
    }

    [Fact]
    public async Task CreateShortLink_ShouldValidateCustomAlias()
    {
        // Arrange
        var user = new ApplicationUser { Id = Guid.NewGuid().ToString(), UserName = "testuser" };
        var request = new CreateShortLinkRequest("https://test.com", "my-alias");

        // Act
        await _service.CreateShortLinkAsync(request, user);
        var act = () => _service.CreateShortLinkAsync(request, user);

        // Assert
        await act.Should().ThrowAsync<ArgumentException>()
            .WithMessage("Этот алиас уже занят");
    }

    [Fact]
    public async Task CreateShortLink_ShouldHashPassword()
    {
        // Arrange
        var user = new ApplicationUser { Id = Guid.NewGuid().ToString(), UserName = "testuser" };
        var request = new CreateShortLinkRequest("https://test.com")
        {
            Password = "secret123"
        };

        // Act
        var link = await _service.CreateShortLinkAsync(request, user);
        
        // Assert
        link.PasswordHash.Should().NotBeNullOrEmpty()
            .And.NotBe("secret123");
        // _service.VerifyPassword(link, "secret123").Should().BeTrue();
    }

    [Fact]
    public async Task CreateShortLink_ShouldExpireLinks()
    {
        // Arrange
        var user = new ApplicationUser { Id = Guid.NewGuid().ToString(), UserName = "testuser" };
        var request = new CreateShortLinkRequest("https://test.com")
        {
            ExpiresAt = DateTime.UtcNow.AddDays(-1)
        };

        // Act
        var link = await _service.CreateShortLinkAsync(request, user);
        
        // Assert
        link.IsExpired.Should().BeTrue();
    }
}