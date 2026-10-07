using Frame.Api.Security;
using Frame.Application.Auth;
using Frame.Application.Auth.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Frame.Api.Controllers.Admin;

/// <summary>
/// Login for the separate admin app. Inherits the admin protection;
/// only the login action itself is open, since nobody has a token yet.
/// Customer accounts are refused with the same INVALID_CREDENTIALS as a wrong password.
/// </summary>
[Route("api/admin/auth")]
public sealed class AdminAuthController : AdminControllerBase
{
    private readonly IAuthService _authService;

    public AdminAuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>Admin login. Limited to 5 attempts per minute.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting(AuthPolicies.LoginRateLimit)]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await _authService.LoginAdminAsync(request, cancellationToken);
        return Ok(response);
    }
}
