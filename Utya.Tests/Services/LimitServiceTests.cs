using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Utya.Data;
using Utya.Models;
using Utya.Services;
using Xunit;
using Microsoft.AspNetCore.Identity;

namespace Utya.Tests.Services;

public class LimitServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ILimitService _limitService;

    public LimitServiceTests()
    {
        // Create a new in-memory database for testing
        var serviceProvider = new ServiceCollection()
            .AddEntityFrameworkInMemoryDatabase()
            .BuildServiceProvider();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid())
            .UseInternalServiceProvider(serviceProvider)
            .Options;

        _context = new ApplicationDbContext(options);
        _limitService = new LimitService(_context);
        
        // Seed the database with test data
        SeedDatabase();
    }

    private void SeedDatabase()
    {
        // Add a test plan
        var freePlan = new Plan
        {
            Id = 1,
            Name = "Бесплатный",
            MaxLinks = 5,
            MaxClicksPerMonth = 1000,
            AllowCustomDomain = false,
            AllowAdvancedAnalytics = false,
            Price = 0
        };

        _context.Plans.Add(freePlan);
        _context.SaveChanges();
    }

    [Fact]
    public async Task CanCreateLinkAsync_WhenUnderLimit_ReturnsTrue()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        
        // Add a user plan with 2 links used out of 5
        _context.UserPlans.Add(new UserPlan
        {
            ApplicationUserId = userId,
            PlanId = 1,
            LinksUsed = 2,
            ClicksUsed = 0,
            IsActive = true,
            ValidUntil = DateTime.UtcNow.AddDays(30)
        });
        await _context.SaveChangesAsync();

        // Act
        var canCreate = await _limitService.CanCreateLinkAsync(userId);

        // Assert
        Assert.True(canCreate);
    }


    [Fact]
    public async Task CanCreateLinkAsync_WhenAtLimit_ReturnsFalse()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        
        // Add a user plan with 5 links used out of 5
        _context.UserPlans.Add(new UserPlan
        {
            ApplicationUserId = userId,
            PlanId = 1,
            LinksUsed = 5, // At limit
            ClicksUsed = 0,
            IsActive = true,
            ValidUntil = DateTime.UtcNow.AddDays(30)
        });
        
        // Add 5 active short links for the user
        for (int i = 0; i < 5; i++)
        {
            _context.ShortLinks.Add(new ShortLink
            {
                OriginalUrl = "https://example.com/" + i,
                ShortCode = "test" + i,
                UserId = userId,
                IsActive = true
            });
        }
        
        await _context.SaveChangesAsync();

        // Act
        var canCreate = await _limitService.CanCreateLinkAsync(userId);

        // Assert
        Assert.False(canCreate);
    }


    [Fact]
    public async Task TrackLinkCreation_IncrementsLinksUsed()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var userPlan = new UserPlan
        {
            ApplicationUserId = userId,
            PlanId = 1,
            LinksUsed = 2,
            ClicksUsed = 0,
            IsActive = true,
            ValidUntil = DateTime.UtcNow.AddDays(30)
        };
        _context.UserPlans.Add(userPlan);
        await _context.SaveChangesAsync();

        // Act
        await _limitService.TrackLinkCreation(userId);

        // Assert
        var updatedPlan = await _context.UserPlans
            .FirstAsync(up => up.ApplicationUserId == userId);
        Assert.Equal(3, updatedPlan.LinksUsed);
    }


    [Fact]
    public async Task GetCurrentLimitsAsync_ReturnsCorrectLimits()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var validUntil = DateTime.UtcNow.AddDays(30);
        
        _context.UserPlans.Add(new UserPlan
        {
            ApplicationUserId = userId,
            PlanId = 1,
            LinksUsed = 2,
            ClicksUsed = 10,
            IsActive = true,
            ValidUntil = validUntil
        });
        await _context.SaveChangesAsync();

        // Act
        var limits = await _limitService.GetCurrentLimitsAsync(userId);

        // Assert
        Assert.Equal("Бесплатный", limits.PlanName);
        Assert.Equal(2, limits.LinksUsed);
        Assert.Equal(5, limits.LinksLimit);
        Assert.Equal(10, limits.ClicksUsed);
        Assert.Equal(1000, limits.ClicksLimit);
        Assert.Equal(validUntil, limits.ValidUntil);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
