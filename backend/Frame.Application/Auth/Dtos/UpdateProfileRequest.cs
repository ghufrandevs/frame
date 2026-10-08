namespace Frame.Application.Auth.Dtos;

/// <summary>Body of PUT /api/auth/me. The email is the login name and cannot be changed.</summary>
public sealed record UpdateProfileRequest(string FullName, string Phone);