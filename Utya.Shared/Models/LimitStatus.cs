namespace Utya.Shared.Models;

public class LimitStatus
{
    public string PlanName { get; set; }
    public object LinksUsed { get; set; }
    public int LinksLimit { get; set; }
    public object ClicksUsed { get; set; }
    public object ValidUntil { get; set; }
    public int ClicksLimit { get; set; }
}