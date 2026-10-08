namespace Frame.Application.Bookings.Dtos;

/// <summary>
/// The selected time. Also the base of CreateBookingRequest, so quote and
/// booking share the same fields and the same validation rules.
/// WithPhotographer is optional: when missing it is false (no photographer).
/// </summary>
public record QuoteRequest
{
    public int? StudioId { get; init; }
    public DateOnly? Date { get; init; }
    public int? StartHour { get; init; }
    public int? EndHour { get; init; }
    public bool WithPhotographer { get; init; }
}