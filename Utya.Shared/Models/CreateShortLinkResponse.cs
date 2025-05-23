namespace Utya.Shared.Models;

public record CreateShortLinkResponse(
    Guid Id,
    string ShortUrl,
    string OriginalUrl,
    DateTime CreatedAt,
    int ClicksCount,
    string? QrCodeBase64 = null
);