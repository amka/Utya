namespace Utya.Data;

public class Plan
{
    public int Id { get; set; }
    public required string Name { get; set; } // "free", "pro", "enterprise"
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int MaxLinks { get; set; }
    public Expiration Expiration { get; set; }
    public int MaxClicksPerMonth { get; set; }
    public bool AllowCustomDomain { get; set; }
    public bool AllowAdvancedAnalytics { get; set; }
}