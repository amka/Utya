using System.Net;

namespace Utya.Services;

public class GeoIpService : IGeoLocator
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<GeoIpService> _logger;

    public GeoIpService(HttpClient httpClient, ILogger<GeoIpService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string?> GetCountryCode(string? ip)
    {
        try
        {
            if (string.IsNullOrEmpty(ip) || ip == "::1" || ip == "127.0.0.1")
                return "RU"; // Для localhost

            var response = await _httpClient.GetFromJsonAsync<GeoIpResponse>(
                $"http://ip-api.com/json/{ip}?fields=countryCode");

            return response?.CountryCode ?? "XX";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка определения геолокации");
            return null;
        }
    }

    private record GeoIpResponse(string CountryCode);
}