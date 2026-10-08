namespace Frame.Application.Auth.Dtos;

/// <summary>
/// Body of POST /api/auth/register.
/// Deliberately has NO Role field: every new account is a Customer,
/// so sending "role": "Admin" in the JSON has no effect at all.
/// </summary>
public sealed record RegisterRequest(
    string FullName,
    string Email,
    string Phone,
    string Password);
