using System.ComponentModel.DataAnnotations.Schema;

namespace Utya.Data;

public class UserPlan
{
    public int Id { get; set; }
    public DateTime ValidUntil { get; set; }
    public int LinksUsed { get; set; }
    public int ClicksUsed { get; set; }
    public bool IsActive { get; set; }
    
    // Foreign Keys
    public string ApplicationUserId { get; set; }
    public ApplicationUser User { get; set; }
    
    public int PlanId { get; set; }
    public Plan Plan { get; set; }
}