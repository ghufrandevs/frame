namespace Frame.Application.Auth.Dtos;

/// <summary>
/// Response of register and both login endpoints (same shape for all three).
/// </summary>
public sealed record AuthResponse(
    string Token,
    DateTimeOffset ExpiresAt,
    AuthUserDto User);

/// <summary>
/// The safe public view of a user: no password hash, no phone.
/// Also returned by GET /api/auth/me.
/// </summary>
public sealed record AuthUserDto(
    int Id,
    string FullName,
    string Email,
    string Role);
