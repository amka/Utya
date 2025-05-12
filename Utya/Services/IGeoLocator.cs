namespace Utya.Services;

public interface IGeoLocator
{
    Task<string?> GetCountryCode(string? ip);
}