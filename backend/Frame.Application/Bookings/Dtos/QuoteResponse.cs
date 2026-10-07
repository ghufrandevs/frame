namespace Frame.Application.Bookings.Dtos;

public sealed record QuoteResponse(
    int StudioId,
    string StudioName,
    DateOnly Date,
    int StartHour,
    int EndHour,
    int Hours,
    decimal HourlyRate,
    decimal Subtotal,
    decimal VatRate,
    decimal VatAmount,
    decimal Total);