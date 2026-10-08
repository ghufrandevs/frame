using System.Text.Json.Serialization;

namespace Frame.Application.Bookings.Dtos;

/// <summary>
/// One row of GET /api/bookings/my: only what the "My bookings" card shows.
/// The full invoice comes from GET /api/bookings/{id}.
/// The admin list also fills CustomerName; for customers it is null and not written.
/// </summary>
public sealed record BookingSummaryResponse(
    int Id,
    string BookingNumber,
    string StudioName,
    DateOnly Date,
    int StartHour,
    int EndHour,
    decimal Total,
    string Status)
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? CustomerName { get; init; }
}