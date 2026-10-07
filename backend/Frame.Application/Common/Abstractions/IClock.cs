namespace Frame.Application.Common.Abstractions;

/// <summary>
/// The only source of "now" in the system.
/// Business rules (past time, opening hours, cancellation) use Muscat time;
/// technical timestamps (PaidAt, CancelledAt, SentAt) use UTC.
/// Services never call DateTime.Now directly, so time can be faked in tests.
/// </summary>
public interface IClock
{
    /// <summary>Current time in UTC, for stored timestamps.</summary>
    DateTime UtcNow { get; }

    /// <summary>Current local time in Muscat (UTC+4), for booking rules.</summary>
    DateTime MuscatNow { get; }

    /// <summary>Today's date in Muscat, e.g. for "can I book today?".</summary>
    DateOnly MuscatToday { get; }
}
