using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utya.Data;
using Utya.Services;
using Utya.Shared.Models;

namespace Utya.Controllers;

public class RedirectController(
    ApplicationDbContext context,
    IGeoLocator geoLocator,
    IClickProcessor clickProcessor,
    ILogger<RedirectController> logger,
    IPasswordHasher<ShortLink> passwordHasher)
    : ControllerBase
{
    [HttpGet("{shortCode}")]
    [ResponseCache(Duration = 60)]
    public async Task<IActionResult> RedirectToOriginal(string shortCode)
    {
        try
        {
            var link = await context.ShortLinks
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.ShortCode == shortCode);

            if (link == null)
            {
                logger.LogWarning("Ссылка не найдена: {ShortCode}", shortCode);
                return NotFound(new { Error = "Ссылка не найдена" });
            }

            // Validate link is active
            if (!link.IsActive)
            {
                logger.LogWarning("Ссылка деактивирована: {Id}", link.Id);
                return StatusCode(410, new { Error = "Ссылка деактивирована" });
            }

            // Validate link expiration
            if (link.ExpiresAt.HasValue && link.ExpiresAt < DateTime.UtcNow)
            {
                logger.LogWarning("Ссылка просрочена: {Id}", link.Id);
                return StatusCode(410, new { Error = "Срок действия ссылки истек" });
            }

            // Password check
            if (!string.IsNullOrEmpty(link.PasswordHash))
            {
                var password = Request.Headers["X-Link-Password"].FirstOrDefault();

                if (string.IsNullOrEmpty(password)
                    || passwordHasher.VerifyHashedPassword(link, link.PasswordHash, password)
                    == PasswordVerificationResult.Failed)
                {
                    logger.LogWarning("Неверный пароль для ссылки: {Id}", link.Id);
                    return StatusCode(403, new { Error = "Требуется пароль" });
                }
            }

            // Collect HttpContext data before the request will be closed
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            var clickData = new ClickData(
                IpAddress: ipAddress,
                UserAgent: HttpContext.Request.Headers.UserAgent,
                Referrer: HttpContext.Request.Headers.Referer,
                // TODO: Move it to the background
                CountryCode: await geoLocator.GetCountryCode(ipAddress)
            );

            // Write impressions async
            clickProcessor.EnqueueClick(link.Id, clickData);

            // Redirect
            // Required to prevent "Invalid non-ASCII or control character in header on redirect"
            var uri = new Uri(link.OriginalUrl);
            return RedirectPreserveMethod(UriHelper.Encode(uri));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Ошибка обработки редиректа для {ShortCode}", shortCode);
            return StatusCode(500, new { Error = "Ошибка обработки запроса" });
        }
    }
}