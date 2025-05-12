using System.ComponentModel.DataAnnotations;

namespace Utya.Models;

public record CreateShortLinkRequest(
    [Required] [Url] string OriginalUrl,
    string? CustomAlias = null,
    string? Password = null,
    DateTime? ExpiresAt = null
);