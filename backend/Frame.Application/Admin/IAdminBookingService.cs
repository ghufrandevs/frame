using Frame.Application.Bookings.Dtos;
using Frame.Application.Common.Dtos;

namespace Frame.Application.Admin;

/// <summary>Bookings for the admin panel: every customer's bookings, and cancellation with refund.</summary>
public interface IAdminBookingService
{
    /// <summary>One page (20) of all bookings, newest first, optionally for one studio and/or one date.</summary>
    Task<PagedResponse<BookingSummaryResponse>> GetPageAsync(
        int? studioId,
        DateOnly? date,
        int page,
        CancellationToken cancellationToken);

    /// <summary>One booking with its invoice and the customer's contact details.</summary>
    Task<BookingResponse> GetAsync(int bookingId, CancellationToken cancellationToken);

    /// <summary>
    /// Cancels before the start, refunds the full amount and emails the customer.
    /// 409 ALREADY_CANCELLED / BOOKING_STARTED / REFUND_FAILED (nothing changed).
    /// </summary>
    Task<BookingResponse> CancelAsync(int bookingId, CancellationToken cancellationToken);
}