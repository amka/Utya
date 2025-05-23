using Utya.Data;
using Utya.Shared.Models;

namespace Utya.Services;

public interface ILimitService
{
    Task<bool> CanCreateLinkAsync(string userId);
    Task<LimitStatus> GetCurrentLimitsAsync(string userId);
    Task TrackLinkCreation(string userId);

    Task<Plan> GetDefaultPlanAsync();
    Task AddUserPlanAsync(UserPlan userPlan);
}