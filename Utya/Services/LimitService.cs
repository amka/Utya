using Microsoft.EntityFrameworkCore;
using Utya.Data;
using Utya.Shared.Models;

namespace Utya.Services;

public class LimitService : ILimitService
{
    private readonly ApplicationDbContext _context;

    public LimitService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CanCreateLinkAsync(string userId)
    {
        var plan = await GetCurrentPlanAsync(userId);
        var currentCount = await _context.ShortLinks
            .CountAsync(l => l.UserId == userId && l.IsActive);

        return currentCount < plan.MaxLinks;
    }

    public async Task<LimitStatus> GetCurrentLimitsAsync(string userId)
    {
        var plan = await GetCurrentPlanAsync(userId);
        
        var usage = await _context.UserPlans
            .FirstOrDefaultAsync(up => up.ApplicationUserId == userId);

        return new LimitStatus
        {
            PlanName = plan.Name,
            LinksUsed = usage?.LinksUsed ?? 0,
            LinksLimit = plan.MaxLinks,
            ClicksUsed = usage?.ClicksUsed ?? 0,
            ClicksLimit = plan.MaxClicksPerMonth,
            ValidUntil = usage?.ValidUntil ?? DateTime.UtcNow.AddDays(30)
        };
    }

    public async Task TrackLinkCreation(string userId)
    {
        var userPlan = await _context.UserPlans
            .FirstOrDefaultAsync(up => up.ApplicationUserId == userId && up.IsActive);

        if (userPlan != null)
        {
            userPlan.LinksUsed++;
            await _context.SaveChangesAsync();
        }
    }

    private async Task<Plan> GetCurrentPlanAsync(string userId)
    {
        return await _context.UserPlans
            .Where(up => up.ApplicationUserId == userId && up.IsActive)
            .Select(up => up.Plan)
            .FirstOrDefaultAsync() ?? await GetDefaultPlanAsync();
    }

    public async Task<Plan> GetDefaultPlanAsync()
    {
        return await _context.Plans
            .FirstAsync(p => p.Name == "Бесплатный");
    }

    public async Task AddUserPlanAsync(UserPlan userPlan)
    {
        _context.UserPlans.Add(userPlan);
        await _context.SaveChangesAsync();
    }
}