using Frame.Application.Bookings.Dtos;

namespace Frame.Application.Bookings;

public interface IBookingService
{
    /// <summary>
    /// Prices a booking before payment. Validates the studio, the date window
    /// and the opening hours, but does not check availability or save anything.
    /// </summary>
    Task<QuoteResponse> QuoteAsync(QuoteRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Charges the card and creates a confirmed booking with its invoice and
    /// confirmation email. userId always comes from the token, never the body.
    /// Errors: 409 SLOT_TAKEN (nothing charged, or refunded at once),
    /// 402 PAYMENT_DECLINED / INSUFFICIENT_FUNDS / PAYMENT_FAILED.
    /// </summary>
    Task<BookingResponse> CreateAsync(int userId, CreateBookingRequest request, CancellationToken cancellationToken);

    /// <summary>All bookings of the customer, newest first.</summary>
    Task<IReadOnlyList<BookingSummaryResponse>> GetMyBookingsAsync(int userId, CancellationToken cancellationToken);

    /// <summary>
    /// One booking with its invoice. Another customer's booking returns
    /// 404 BOOKING_NOT_FOUND, so ids cannot be probed.
    /// </summary>
    Task<BookingResponse> GetMyBookingAsync(int userId, int bookingId, CancellationToken cancellationToken);
}