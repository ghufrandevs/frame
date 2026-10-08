using Frame.Application.Common.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace Frame.Infrastructure.Security;

/// <summary>
/// IPasswordHasher implemented with Microsoft's ASP.NET Core Identity hasher
/// (PBKDF2 with a random salt per password, many iterations).
/// We reuse a proven, well-reviewed algorithm instead of writing our own crypto.
/// </summary>
internal sealed class IdentityPasswordHasher : IPasswordHasher
{
    // The Identity hasher needs a "user" type, but its algorithm never reads it.
    private static readonly object NoUser = new();
    private readonly PasswordHasher<object> _hasher = new();

    public string Hash(string password)
    {
        if (string.IsNullOrEmpty(password))
            throw new ArgumentException("Password is required.", nameof(password));

        return _hasher.HashPassword(NoUser, password);
    }

    public bool Verify(string hashedPassword, string providedPassword)
    {
        if (string.IsNullOrEmpty(hashedPassword) || string.IsNullOrEmpty(providedPassword))
            return false;

        var result = _hasher.VerifyHashedPassword(NoUser, hashedPassword, providedPassword);

        // SuccessRehashNeeded = correct password, but hashed with older settings. Still a valid login.
        return result is PasswordVerificationResult.Success
                      or PasswordVerificationResult.SuccessRehashNeeded;
    }
}
