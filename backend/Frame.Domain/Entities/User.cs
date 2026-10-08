using Frame.Domain.Common;
using Frame.Domain.Enums;

namespace Frame.Domain.Entities;

/// <summary>
/// A person with an account: a customer who books, or an admin who manages.
/// Created only through the factory methods below, so a user
/// can never exist without a name, a normalized email and a hashed password.
/// The email is the login name and never changes.
/// </summary>
public sealed class User : BaseEntity
{
    public string FullName { get; private set; } = null!;

    /// <summary>Always stored trimmed and lower-case, so "A@x.com" and "a@x.com" are the same account.</summary>
    public string Email { get; private set; } = null!;

    public string Phone { get; private set; } = null!;

    /// <summary>Only the hash is stored. The plain password never reaches the Domain.</summary>
    public string PasswordHash { get; private set; } = null!;

    public UserRole Role { get; private set; }

    /// <summary>Required by EF Core to load users from the database. Not for application code.</summary>
    private User() { }

    private User(string fullName, string email, string phone, string passwordHash, UserRole role)
    {
        FullName = Guard.NotEmpty(fullName, "FULL_NAME_REQUIRED");
        Email = NormalizeEmail(email);
        Phone = Guard.NotEmpty(phone, "PHONE_REQUIRED");
        PasswordHash = Guard.NotEmpty(passwordHash, "PASSWORD_HASH_REQUIRED");
        Role = role;
    }

    /// <summary>Used by sign-up. The role is always Customer, whatever the request contains.</summary>
    public static User CreateCustomer(string fullName, string email, string phone, string passwordHash)
        => new(fullName, email, phone, passwordHash, UserRole.Customer);

    /// <summary>Used only by the database seed to create the admin account.</summary>
    public static User CreateAdmin(string fullName, string email, string phone, string passwordHash)
        => new(fullName, email, phone, passwordHash, UserRole.Admin);

    /// <summary>Profile page: name and phone only. The email (login name) stays the same.</summary>
    public void UpdateProfile(string fullName, string phone)
    {
        FullName = Guard.NotEmpty(fullName, "FULL_NAME_REQUIRED");
        Phone = Guard.NotEmpty(phone, "PHONE_REQUIRED");
    }

    /// <summary>
    /// One place that defines how emails are compared.
    /// The login service uses it too, before searching for the user.
    /// </summary>
    public static string NormalizeEmail(string? email)
        => Guard.NotEmpty(email, "EMAIL_REQUIRED").ToLowerInvariant();
}