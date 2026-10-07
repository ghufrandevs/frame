namespace Frame.Domain.Enums;

/// <summary>
/// The only two states saved in the database.
/// Upcoming / InProgress / Completed are NOT stored: they are
/// calculated from the current Muscat time when the booking is displayed.
/// </summary>
public enum BookingStatus
{
    /// <summary>Paid and confirmed. Every booking starts in this state.</summary>
    Confirmed = 1,

    /// <summary>Cancelled by an admin before it started, with a full refund.</summary>
    Cancelled = 2
}
