namespace Utya.Shared.Models;

public record ClickData(
    string? IpAddress,
    string? UserAgent,
    string? Referrer,
    string? CountryCode);