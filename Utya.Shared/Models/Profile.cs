namespace Utya.Shared.Models;

public class Profile
{
    public string? UserName {get; set;}
    public string? Email {get; set;}
    public LimitStatus Limits {get; set;}
}