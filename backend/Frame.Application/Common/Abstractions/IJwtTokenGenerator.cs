using Frame.Domain.Entities;

namespace Frame.Application.Common.Abstractions;

/// <summary>
/// Issues the signed JWT a user sends with every request after login.
/// The token carries the user id, name and role, and an audience that
/// matches the role: customer tokens work only on customer routes,
/// admin tokens only on /api/admin routes.
/// </summary>
public interface IJwtTokenGenerator
{
    AccessToken Generate(User user);
}

/// <summary>
/// A token and the moment it stops working.
/// ExpiresAt carries the Muscat offset (+04:00), as in the API contract.
/// </summary>
public sealed record AccessToken(string Value, DateTimeOffset ExpiresAt);
