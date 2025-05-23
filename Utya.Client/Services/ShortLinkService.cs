using System.Net.Http.Json;
using Utya.Shared.Models;
using Utya.Shared.Services;

namespace Utya.Client.Services;

public class ShortLinkService(IHttpClientFactory clientFactory, ILogger<ShortLinkService> logger) : IShortLinkService
{
    public Task<ShortLinkDto> CreateShortLinkAsync(CreateShortLinkRequest request, string userId)
    {
        throw new NotImplementedException();
    }

    public Task<ShortLinkDto?> GetLinkAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<ShortLinkDto>> GetLinksAsync(int page, int perPage, string? user)
    {
        try
        {
            var client = clientFactory.CreateClient("Utya.ServerAPI");
            var items = await client.GetFromJsonAsync<List<ShortLinkDto>>(
                $"api/v1/links?page={page}&perPage={perPage}");
            return items ?? [];
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return [];
        }
    }
}