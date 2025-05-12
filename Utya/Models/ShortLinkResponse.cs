namespace Utya.Models;

public record ShortLinkResponse(
    String Id,
    string ShortUrl,
    string OriginalUrl,
    DateTime CreatedAt,
    int ClicksCount,
    string? QrCodeBase64 = null
);