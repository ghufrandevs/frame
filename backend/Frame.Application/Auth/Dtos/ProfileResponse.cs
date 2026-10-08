namespace Frame.Application.Auth.Dtos;

/// <summary>
/// GET and PUT /api/auth/me: the profile page. Same fields as the login user
/// plus the phone (8 digits, stored without +968).
/// </summary>
public sealed record ProfileResponse(
    int Id,
    string FullName,
    string Email,
    string Phone,
    string Role);