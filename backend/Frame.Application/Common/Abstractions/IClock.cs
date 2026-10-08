namespace Frame.Application.Common.Abstractions;

/// <summary>
/// The only source of "now" in the system, and the only place that knows
/// Muscat's offset. Business rules (past time, opening hours, cancellation)
/// use Muscat time; technical timestamps (PaidAt, CancelledAt, SentAt) are
/// stored in UTC and converted with ToMuscat when shown.
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

    /// <summary>A stored UTC timestamp as Muscat time with its offset, e.g. 2026-10-06T08:45:00+04:00.</summary>
    DateTimeOffset ToMuscat(DateTime utc);
}