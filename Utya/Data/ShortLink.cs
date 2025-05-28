using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using NanoidDotNet;

namespace Utya.Data;

public class ShortLink
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [Url]
    [MaxLength(2000)]
    public string OriginalUrl { get; set; } = null!;

    [Required]
    [MaxLength(10)]
    public string ShortCode { get; set; } = null!;

    [MaxLength(50)]
    public string? CustomAlias { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAt { get; set; }
    
    [MaxLength(100)]
    public string? PasswordHash { get; set; }
    public bool IsActive { get; set; } = true;

    // Foreign keys
    public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }

    // Navigation properties
    public List<Click> Clicks { get; } = new();
    // public List<GeoRedirect> GeoRedirects { get; } = new();
    
    public bool IsExpired => ExpiresAt < DateTime.UtcNow;
}