namespace Frame.Application.Bookings.Dtos;

/// <summary>
/// Price summary before payment. PhotographerRatePerHour is always returned so the
/// page can show the add-on price; PhotographerFee is 0 when it is not chosen.
/// </summary>
public sealed record QuoteResponse(
    int StudioId,
    string StudioName,
    DateOnly Date,
    int StartHour,
    int EndHour,
    int Hours,
    decimal HourlyRate,
    decimal PhotographerRatePerHour,
    decimal PhotographerFee,
    decimal Subtotal,
    decimal VatRate,
    decimal VatAmount,
    decimal Total);