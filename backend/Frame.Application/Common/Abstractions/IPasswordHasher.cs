namespace Frame.Application.Common.Abstractions;

/// <summary>
/// Turns a plain password into a one-way hash and checks a login attempt against it.
/// Application code depends on this interface only; the real algorithm
/// lives in Infrastructure and can change without touching any service.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>Returns a salted hash that is safe to store. The same password gives a different hash every time.</summary>
    string Hash(string password);

    /// <summary>True if the provided password matches the stored hash.</summary>
    bool Verify(string hashedPassword, string providedPassword);
}
