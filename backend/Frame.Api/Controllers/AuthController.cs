using Frame.Api.Extensions;
using Frame.Api.Security;
using Frame.Application.Auth;
using Frame.Application.Auth.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Frame.Api.Controllers;

/// <summary>
/// Customer sign-up, login and profile. Thin by design: receives the
/// request (already validated by ValidationFilter), calls the service,
/// returns the result. No business logic lives here.
/// </summary>
[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>Creates a customer account and returns a token (logged in right away).</summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var response = await _authService.RegisterAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    /// <summary>Customer login. Limited to 5 attempts per minute.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [EnableRateLimiting(AuthPolicies.LoginRateLimit)]
    [ProducesResponseType<AuthResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken cancellationToken)
    {
        var response = await _authService.LoginCustomerAsync(request, cancellationToken);
        return Ok(response);
    }

    /// <summary>The logged-in customer's profile, for the top bar and the profile page.</summary>
    [HttpGet("me")]
    [Authorize(Policy = AuthPolicies.Customer)]
    [ProducesResponseType<ProfileResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Me(CancellationToken cancellationToken)
    {
        var profile = await _authService.GetCurrentUserAsync(User.GetUserId(), cancellationToken);
        return Ok(profile);
    }

    /// <summary>Changes the customer's name and phone. The email cannot be changed.</summary>
    [HttpPut("me")]
    [Authorize(Policy = AuthPolicies.Customer)]
    [ProducesResponseType<ProfileResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateMe(UpdateProfileRequest request, CancellationToken cancellationToken)
    {
        var profile = await _authService.UpdateProfileAsync(User.GetUserId(), request, cancellationToken);
        return Ok(profile);
    }
}