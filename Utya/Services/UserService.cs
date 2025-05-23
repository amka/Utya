using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Utya.Data;
using Utya.Shared.Models;
using Utya.Shared.Services;

namespace Utya.Services;

public class UserService(
    ApplicationDbContext context,
    ILimitService limitService,
    AuthenticationStateProvider authenticationStateAsync) : IUserService
{
    public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
    {
        return await context.Users
            .Include(u => u.ShortLinks)
            .FirstOrDefaultAsync(u => u.Id == userId);
    }

    public async Task<LimitStatus> GetUserLimitsAsync(string userId)
    {
        return await limitService.GetCurrentLimitsAsync(userId);
    }

    public async Task<bool> CanUserCreateLinkAsync(string userId)
    {
        return await limitService.CanCreateLinkAsync(userId);
    }

    public async Task<Profile?> GetProfileAsync(string userId)
    {
        var user = await GetUserByIdAsync(userId);
        if (user == null) return null;
        return new Profile()
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Limits = await limitService.GetCurrentLimitsAsync(user.Id)
        };
    }

    public async Task<string?> GetCurrentUserId()
    {
        var authState = await authenticationStateAsync.GetAuthenticationStateAsync();
        var user = authState.User;
        return user.FindFirst(u => u.Type.Contains("nameidentifier"))?.Value;
    }
}