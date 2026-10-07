namespace Frame.Application.Bookings.Dtos;

/// <summary>
/// One row of GET /api/bookings/my: only what the "My bookings" card shows.
/// The full invoice comes from GET /api/bookings/{id}.
/// </summary>
public sealed record BookingSummaryResponse(
    int Id,
    string BookingNumber,
    string StudioName,
    DateOnly Date,
    int StartHour,
    int EndHour,
    decimal Total,
    string Status);