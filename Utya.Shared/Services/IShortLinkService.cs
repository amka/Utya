using System.Security.Claims;
using Utya.Shared.Models;

namespace Utya.Shared.Services;

public interface IShortLinkService
{
    Task<ShortLinkDto> CreateShortLinkAsync(
        CreateShortLinkRequest request,
        string userId);

    Task<ShortLinkDto?> GetLinkAsync(Guid id);
    Task<List<ShortLinkDto>> GetLinksAsync(int page, int perPage, string user);
}