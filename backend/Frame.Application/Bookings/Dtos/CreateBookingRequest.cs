namespace Frame.Application.Bookings.Dtos;

/// <summary>
/// POST /api/bookings: the selected time (inherited from QuoteRequest)
/// plus the payment token from the provider. There is no card number,
/// CVV or amount field on purpose: they can never reach the server.
/// </summary>
public sealed record CreateBookingRequest : QuoteRequest
{
    public string? PaymentToken { get; init; }
}