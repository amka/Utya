using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Utya.Data;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    [MaxLength(100)]
    public string? CustomDomain { get; set; }
    
    // Navigation property
    public List<ShortLink> ShortLinks { get; } = [];
    
    public int? PlanId { get; set; }
    public Plan? Plan { get; set; } 
}

