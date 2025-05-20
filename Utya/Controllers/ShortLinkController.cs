using System.Drawing;
using System.Text;
using QRCoder;
using Utya.Data;
using Utya.Models;
using Utya.Services;

namespace Utya.Controllers;

// Controllers/ShortLinkController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/v1/links")]
public class ShortLinkController : ControllerBase
{
    private readonly ShortLinkService _shortLinkService;
    private readonly ILogger<ShortLinkController> _logger;
    private readonly IWebHostEnvironment _env;

    public ShortLinkController(
        ShortLinkService shortLinkService,
        ILogger<ShortLinkController> logger,
        IWebHostEnvironment env)
    {
        _shortLinkService = shortLinkService;
        _logger = logger;
        _env = env;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateShortLinkResponse), 201)]
    [ProducesResponseType(400)]
    [ProducesResponseType(409)]
    public async Task<IActionResult> CreateShortLink(
        [FromBody] CreateShortLinkRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = User.Identity is { IsAuthenticated: true }
                ? await _shortLinkService.GetCurrentUserAsync(User)
                : null;

            var shortLink = await _shortLinkService.CreateShortLinkAsync(request, user);

            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            var response = new CreateShortLinkResponse(
                Id: shortLink.Id,
                ShortUrl: $"{baseUrl}/{shortLink.ShortCode}",
                OriginalUrl: shortLink.OriginalUrl,
                CreatedAt: shortLink.CreatedAt,
                ClicksCount: 0,
                QrCodeBase64: GenerateQrCodeAsync($"{baseUrl}/{shortLink.ShortCode}")
            );

            return CreatedAtAction(nameof(GetLink), new { id = shortLink.Id }, response);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Ошибка создания ссылки");
            return Conflict(new { Error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании ссылки");
            return StatusCode(500, new { Error = "Внутренняя ошибка сервера" });
        }
    }

    private string? GenerateQrCodeAsync(string url)
    {
        if (!_env.IsDevelopment()) return null;

        // Генерируем QR только в development
        using var qrGenerator = new QRCodeGenerator();
        var qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
        var qrCode = new SvgQRCode(qrCodeData);
        var bytes = Encoding.UTF8.GetBytes(qrCode.GetGraphic(new Size(96, 96), false));
        return $"data:image/svg+xml;base64,{Convert.ToBase64String(bytes)}";
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(CreateShortLinkResponse), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetLink(Guid id)
    {
        var link = await _shortLinkService.GetLinkAsync(id);
        return link != null ? Ok(link) : NotFound();
    }
    
    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(List<ShortLink>), 200)]
    public async Task<IActionResult> GetLinks([FromQuery]int page, [FromQuery]int perPage, [FromQuery]string? search)
    {
        var user = User.Identity is { IsAuthenticated: true }
            ? await _shortLinkService.GetCurrentUserAsync(User)
            : null;

        if (user == null) return Unauthorized();
        
        var links = await _shortLinkService.GetLinksAsync(page, perPage, user);
        return Ok(links);
    }
}