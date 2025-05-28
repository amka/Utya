using Microsoft.AspNetCore.Identity;
using Utya.Data;

namespace Utya.Services;

public class LogEmailSender(ILogger<LogEmailSender> logger) : IEmailSender<ApplicationUser>
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        logger.LogInformation("Sending email to {Email}", email);
        logger.LogInformation("Subject: {Subject}", subject);
        logger.LogInformation("Message: {Message}", htmlMessage);
        return Task.CompletedTask;
    }

    public Task SendConfirmationLinkAsync(ApplicationUser user, string email, string confirmationLink)
    {
        SendEmailAsync(email, "Confirm your email", $"Please confirm your account by <a href='{confirmationLink}'>clicking here</a>.");
        return Task.CompletedTask;
    }

    public Task SendPasswordResetLinkAsync(ApplicationUser user, string email, string resetLink)
    {
        SendEmailAsync(email, "Reset your password", $"Please reset your password by <a href='{resetLink}'>clicking here</a>.");
        return Task.CompletedTask;
    }

    public Task SendPasswordResetCodeAsync(ApplicationUser user, string email, string resetCode)
    {
        SendEmailAsync(email, "Reset your password", $"Please reset your password using the following code: {resetCode}");
        return Task.CompletedTask;
    }
}