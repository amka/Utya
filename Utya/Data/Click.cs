using System.ComponentModel.DataAnnotations;

namespace Utya.Data;

public class Click
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    
    [MaxLength(45)]
    public string? IpAddress { get; set; }
    
    [MaxLength(500)]
    public string? UserAgent { get; set; }
    
    [MaxLength(500)]
    public string? Referrer { get; set; }
    
    [MaxLength(2)]
    public string? CountryCode { get; set; }
    
    // Foreign keys
    public ShortLink ShortLink { get; set; } = null!;
}