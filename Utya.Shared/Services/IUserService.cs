using Utya.Shared.Models;

namespace Utya.Shared.Services;

public interface IUserService
{
    // Task<ApplicationUser?> GetUserByIdAsync(string userId);
    // Task<LimitStatus> GetUserLimitsAsync(string userId);
    // Task<bool> CanUserCreateLinkAsync(string userId);
    Task<Profile?> GetProfileAsync(string userId);
    Task<string?> GetCurrentUserId();
}