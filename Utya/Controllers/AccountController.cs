using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Utya.Data;
using Utya.Services;
using Utya.Shared.Services;

namespace Utya.Controllers;

[ApiController]
[Authorize]
[Route("api/v1/[controller]")]
public class AccountController(
    UserManager<ApplicationUser> userManager,
    ILogger<AccountController> logger,
    IUserService userService,
    ILimitService limitService)
    : ControllerBase
{
    [HttpGet("profile")]
    public async Task<IActionResult> Get()
    {
        var user = User.Identity is { IsAuthenticated: true }
            ? await userManager.GetUserAsync(User)
            : null;

        if (user == null) return Unauthorized();


        return Ok(await userService.GetProfileAsync(user.Id));
    }

    [HttpGet("/Logout")]
    public async Task Logout([FromQuery] string returnUrl = "/")
    {
        await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
        HttpContext.Response.Redirect(returnUrl);
    }
}