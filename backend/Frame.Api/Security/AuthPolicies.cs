namespace Frame.Api.Security;

/// <summary>
/// Names used in [Authorize(Policy = ...)] and [EnableRateLimiting(...)].
/// The rules behind each name are defined once in Program.cs.
/// </summary>
internal static class AuthPolicies
{
    /// <summary>Role Customer AND a token issued for the customer app (aud = frame-web).</summary>
    public const string Customer = "CustomerOnly";

    /// <summary>Role Admin AND a token issued for the admin app (aud = frame-admin).</summary>
    public const string Admin = "AdminOnly";

    /// <summary>Rate limit for both login endpoints: 5 attempts per minute per IP.</summary>
    public const string LoginRateLimit = "login";
}
