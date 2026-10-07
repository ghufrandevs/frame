using Frame.Domain.Entities;

namespace Frame.Application.Common.Abstractions.Persistence;

/// <summary>
/// Data access for bookings: availability reads, the clash check,
/// the booking number sequence, and adding new bookings.
/// </summary>
public interface IBookingRepository
{
    /// <summary>
    /// Confirmed (not cancelled) bookings of one studio between two dates, both included.
    /// One query serves a single day (from = to) or a whole month. Read-only.
    /// </summary>
    Task<IReadOnlyList<Booking>> GetConfirmedForStudioAsync(
        int studioId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// True if a confirmed booking of this studio on this date overlaps
    /// [startHour, endHour). Inside a Serializable transaction this read also
    /// locks the range, so no one can insert a clashing booking until commit.
    /// </summary>
    Task<bool> HasOverlapAsync(
        int studioId,
        DateOnly date,
        int startHour,
        int endHour,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Next value of the BookingNumbers sequence (1001, 1002, ...).
    /// The database guarantees a unique number even under concurrent requests.
    /// </summary>
    Task<int> NextBookingSequenceAsync(CancellationToken cancellationToken = default);

    /// <summary>Tracks a new booking. Written to the database by IUnitOfWork.</summary>
    void Add(Booking booking);
}