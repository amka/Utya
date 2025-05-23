using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NanoidDotNet;
using Utya.Data;
using Utya.Models;

namespace Utya.Services;

public class ShortLinkService(
    ApplicationDbContext context,
    IPasswordHasher<ShortLink> hasher,
    IGeoLocator geoLocator,
    ILogger<ShortLinkService> logger,
    UserManager<ApplicationUser> userManager,
    ILimitService limitService)
{
    public async Task<ShortLink> CreateShortLinkAsync(
        CreateShortLinkRequest request,
        ApplicationUser? user = null)
    {
        if (!string.IsNullOrEmpty(request.CustomAlias))
        {
            if (await context.ShortLinks.AnyAsync(s => s.CustomAlias == request.CustomAlias))
            {
                throw new ArgumentException("Этот алиас уже занят");
            }

            // request.CustomAlias = request.CustomAlias.ToLowerInvariant();
        }

        // Check if user can create more links
        if (user != null && !await limitService.CanCreateLinkAsync(user.Id))
        {
            throw new InvalidOperationException("Вы достигли лимита по количеству ссылок для вашего тарифного плана");
        }

        var shortLink = new ShortLink
        {
            OriginalUrl = request.OriginalUrl,
            // ShortCode = await GenerateUniqueCode(),
            ShortCode = await GenerateShortCode(),
            CustomAlias = request.CustomAlias,
            ExpiresAt = request.ExpiresAt,
            User = user
        };

        if (!string.IsNullOrEmpty(request.Password))
        {
            shortLink.PasswordHash = hasher.HashPassword(shortLink, request.Password);
        }

        await context.ShortLinks.AddAsync(shortLink);
        await context.SaveChangesAsync();

        // Track link creation for the user
        if (user != null)
        {
            await limitService.TrackLinkCreation(user.Id);
        }

        return shortLink;
    }

    public async Task<ShortLink?> GetLinkAsync(Guid id)
    {
        return await context.ShortLinks
            .Include(s => s.Clicks)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<ApplicationUser?> GetCurrentUserAsync(ClaimsPrincipal principal)
    {
        // var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        // if (userId == null || !Guid.TryParse(userId, out var guid))
        //     return null;
        return await userManager.GetUserAsync(principal);
    }

    private async Task<string> GenerateUniqueCode(int length = 6)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var code = new StringBuilder();
        var retries = 0;

        do
        {
            code.Clear();
            for (int i = 0; i < length; i++)
            {
                code.Append(chars[Random.Shared.Next(chars.Length)]);
            }

            if (retries++ > 5) throw new InvalidOperationException("Failed to generate unique code");
        } while (await context.ShortLinks.AnyAsync(s => s.ShortCode == code.ToString()));

        return code.ToString();
    }

    private async Task<string> GenerateShortCode(int length = 6)
    {
        var longCode = await Nanoid.GenerateAsync();
        return longCode[..length];
    }

    public async Task RecordClick(ShortLink link, HttpContext httpContext)
    {
        var ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();
        var click = new Click
        {
            ShortLink = link,
            IpAddress = ipAddress,
            UserAgent = httpContext.Request.Headers.UserAgent,
            Referrer = httpContext.Request.Headers.Referer,
            CountryCode = await geoLocator.GetCountryCode(ipAddress)
        };

        await context.Clicks.AddAsync(click);
        await context.SaveChangesAsync();
    }

    public async Task<List<ShortLink>> GetLinksAsync(int page, int perPage, ApplicationUser? user)
    {
        return await context.ShortLinks
            .AsNoTracking()
            .Where(l => l.User == user)
            .OrderByDescending(l => l.CreatedAt)
            .Include(s => s.Clicks)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync();
    }
}