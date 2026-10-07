using Frame.Application.Bookings.Dtos;

namespace Frame.Application.Bookings;

public interface IBookingService
{
    /// <summary>
    /// Prices a booking before payment. Validates the studio, the date window
    /// and the opening hours, but does not check availability or save anything.
    /// </summary>
    Task<QuoteResponse> QuoteAsync(QuoteRequest request, CancellationToken cancellationToken);
}