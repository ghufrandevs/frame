namespace Frame.Application.Auth.Dtos;

/// <summary>
/// Body of POST /api/auth/login and POST /api/admin/auth/login.
/// Same shape for both; the service decides which roles each one accepts.
/// </summary>
public sealed record LoginRequest(
    string Email,
    string Password);
