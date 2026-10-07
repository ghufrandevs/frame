namespace Frame.Application.Bookings.Dtos;

public sealed record QuoteRequest
{
    public int? StudioId { get; init; }
    public DateOnly? Date { get; init; }
    public int? StartHour { get; init; }
    public int? EndHour { get; init; }
}