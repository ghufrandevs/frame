using Frame.Domain.Entities;

namespace Frame.Application.Common.Abstractions.Persistence;

/// <summary>
/// Data access for bookings: availability reads, the clash check,
/// the booking number sequence, customer and admin reads, dashboard reads,
/// and adding new bookings.
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

    /// <summary>All bookings of one customer with their studio, newest first. Read-only.</summary>
    Task<IReadOnlyList<Booking>> GetForUserAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>One booking with its studio and customer, or null. Read-only.</summary>
    Task<Booking?> GetByIdWithDetailsAsync(int bookingId, CancellationToken cancellationToken = default);

    /// <summary>
    /// One page of all bookings for the admin, newest first, with studio and customer.
    /// Optional filters by studio and date. TotalCount counts every match, not just this page.
    /// </summary>
    Task<(IReadOnlyList<Booking> Items, int TotalCount)> GetAdminPageAsync(
        int? studioId,
        DateOnly? date,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    /// <summary>One booking with its studio and customer, TRACKED so it can be changed (admin cancel).</summary>
    Task<Booking?> GetByIdForUpdateAsync(int bookingId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Confirmed bookings of every studio between two dates, both included, with studio
    /// and customer, ordered by date and hour. Dashboard: occupancy and today's list. Read-only.
    /// </summary>
    Task<IReadOnlyList<Booking>> GetConfirmedBetweenAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken = default);

    /// <summary>
    /// Bookings paid or cancelled since a UTC time, with their studio. Dashboard: revenue,
    /// count and refunds. The caller narrows to the Muscat month with IClock.ToMuscat. Read-only.
    /// </summary>
    Task<IReadOnlyList<Booking>> GetPaidOrCancelledSinceAsync(DateTime sinceUtc, CancellationToken cancellationToken = default);

    /// <summary>Tracks a new booking. Written to the database by IUnitOfWork.</summary>
    void Add(Booking booking);
}