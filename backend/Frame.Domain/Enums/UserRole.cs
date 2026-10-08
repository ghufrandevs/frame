namespace Frame.Domain.Enums;

/// <summary>
/// What a user is allowed to do in the system.
/// Stored in the database as text ("Customer" / "Admin").
/// </summary>
public enum UserRole
{
    /// <summary>Books studios and sees own bookings. Every new account gets this role.</summary>
    Customer = 1,

    /// <summary>Manages studios and bookings. Created by the seed only, never by sign-up.</summary>
    Admin = 2
}