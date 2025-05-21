using System.ComponentModel.DataAnnotations;

namespace Utya.Shared.Models;

public class ShortLinkFormModel
{
    [Required]
    [StringLength(2000, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
    [Url(ErrorMessage = "{0} is not a valid URL")]
    public string? Url { get; set; }

    [StringLength(32, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
    public string? Password { get; set; }

    public string? CustomAlias { get; set; }

    public ShortLinkExpiration? Expiration { get; set; } // Ensure this uses the new enum name
}
