using System.Security.Cryptography;
using System.Text;

namespace Utya.Shared.Models;

public class Profile
{
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public LimitStatus Limits { get; set; }

    public string GravatarUrl => GetGravatarUrl(Email);

    private static string GetGravatarUrl(string? email)
    {
        if (string.IsNullOrEmpty(email))
        {
            return string.Empty;
        }

        // Normalize the email address: trim and convert to lowercase
        email = email.Trim().ToLower();
        var emailBytes = Encoding.UTF8.GetBytes(email);

        var sb = new StringBuilder();
        foreach (var b in SHA256.HashData(emailBytes))
        {
            sb.Append(b.ToString("x2"));
        }

        var gravatarUrl = $"https://www.gravatar.com/avatar/{sb}?d=initials&name={email}";

        return gravatarUrl;
    }
}