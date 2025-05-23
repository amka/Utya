using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utya.Data;
using Utya.Services;
using Utya.Shared.Models;

namespace Utya.Controllers;

public class RedirectController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IGeoLocator _geoLocator;
    private readonly ILogger<RedirectController> _logger;
    private readonly IPasswordHasher<ShortLink> _passwordHasher;

    private readonly IClickProcessor _clickProcessor;

    public RedirectController(
        ApplicationDbContext context,
        IGeoLocator geoLocator,
        IClickProcessor clickProcessor,
        ILogger<RedirectController> logger,
        IPasswordHasher<ShortLink> passwordHasher
    )
    {
        _context = context;
        _geoLocator = geoLocator;
        _logger = logger;
        _passwordHasher = passwordHasher;
        _clickProcessor = clickProcessor;
    }

    [HttpGet("{shortCode}")]
    [ResponseCache(Duration = 60)]
    public async Task<IActionResult> RedirectToOriginal(string shortCode)
    {
        try
        {
            var link = await _context.ShortLinks
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.ShortCode == shortCode);

            if (link == null)
            {
                _logger.LogWarning("Ссылка не найдена: {ShortCode}", shortCode);
                return NotFound(new { Error = "Ссылка не найдена" });
            }

            // Validate link is active
            if (!link.IsActive)
            {
                _logger.LogWarning("Ссылка деактивирована: {Id}", link.Id);
                return StatusCode(410, new { Error = "Ссылка деактивирована" });
            }

            // Validate link expiration
            if (link.ExpiresAt.HasValue && link.ExpiresAt < DateTime.UtcNow)
            {
                _logger.LogWarning("Ссылка просрочена: {Id}", link.Id);
                return StatusCode(410, new { Error = "Срок действия ссылки истек" });
            }

            // Password check
            if (!string.IsNullOrEmpty(link.PasswordHash))
            {
                var password = Request.Headers["X-Link-Password"].FirstOrDefault();

                if (string.IsNullOrEmpty(password)
                    || _passwordHasher.VerifyHashedPassword(link, link.PasswordHash, password)
                    == PasswordVerificationResult.Failed)
                {
                    _logger.LogWarning("Неверный пароль для ссылки: {Id}", link.Id);
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
                CountryCode: await _geoLocator.GetCountryCode(ipAddress)
            );

            // Write impressions async
            _clickProcessor.EnqueueClick(link.Id, clickData);

            // Redirect
            return RedirectPreserveMethod(link.OriginalUrl);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка обработки редиректа для {ShortCode}", shortCode);
            return StatusCode(500, new { Error = "Ошибка обработки запроса" });
        }
    }
}