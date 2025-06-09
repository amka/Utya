using System.Net.Http.Json;
using Utya.Shared.Models;
using Utya.Shared.Services;

namespace Utya.Client.Services;

public class ShortLinkService(IHttpClientFactory clientFactory, ILogger<ShortLinkService> logger) : IShortLinkService
{
    public async Task<ShortLinkDto> CreateShortLinkAsync(CreateShortLinkRequest request, string? userId)
    {
        logger.LogInformation("Creating new ShortLink");
        
        var client = clientFactory.CreateClient("Utya.ServerAPI");
        var response = await client.PostAsJsonAsync<CreateShortLinkRequest>("api/v1/links", request);
        
        logger.LogInformation("Response: {Response}", response);
        
        if (!response.IsSuccessStatusCode) throw new Exception("Failed to create ShortLink");
        
        var result = await response.Content.ReadFromJsonAsync<ShortLinkDto>();
        return result ?? throw new Exception("Failed to create ShortLink");

    }

    public Task<ShortLinkDto?> GetLinkAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<ShortLinkDto>> GetLinksAsync(int page, int perPage, string? user, string? search = null)
    {
        try
        {
            var client = clientFactory.CreateClient("Utya.ServerAPI");
            var url = $"api/v1/links?page={page}&perPage={perPage}";
            
            if (!string.IsNullOrWhiteSpace(search))
            {
                url += $"&search={Uri.EscapeDataString(search)}";
            }
            
            var items = await client.GetFromJsonAsync<List<ShortLinkDto>>(url);
            return items ?? [];
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return [];
        }
    }
}