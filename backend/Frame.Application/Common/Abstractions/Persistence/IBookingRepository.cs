using Frame.Domain.Entities;

namespace Frame.Application.Common.Abstractions.Persistence;

/// <summary>
/// Data access for bookings. Starts with what availability needs;
/// creating and listing bookings are added with the booking feature.
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
}
