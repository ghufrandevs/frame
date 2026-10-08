using System.Text.Json.Serialization;

namespace Frame.Application.Bookings.Dtos;

/// <summary>
/// One booking with its invoice, as returned by POST /api/bookings and
/// GET /api/bookings/{id}. Each nested object matches one block on the page.
/// Times are Muscat time with the offset (+04:00).
/// Admin endpoints also fill Customer; for customers it is null and not written at all.
/// </summary>
public sealed record BookingResponse(
    int Id,
    string BookingNumber,
    string Status,
    BookingStudioDto Studio,
    DateOnly Date,
    int StartHour,
    int EndHour,
    InvoiceDto Invoice,
    PaymentDto Payment,
    CancellationDto? Cancellation)
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public CustomerDto? Customer { get; init; }
}

public sealed record BookingStudioDto(int Id, string Name, string ImageUrl);

public sealed record InvoiceDto(
    string InvoiceNumber,
    DateTimeOffset IssuedAt,
    string CustomerName,
    int Hours,
    decimal HourlyRate,
    decimal Subtotal,
    decimal VatAmount,
    decimal Total);

public sealed record PaymentDto(string Reference, string CardBrand, string CardLast4, DateTimeOffset PaidAt);

/// <summary>Filled only when an admin cancelled the booking; otherwise null.</summary>
public sealed record CancellationDto(DateTimeOffset CancelledAt, decimal RefundAmount);

/// <summary>Contact details of the customer, for admin screens only.</summary>
public sealed record CustomerDto(string FullName, string Email, string Phone);

/// <summary>
/// Status shown to the customer, computed from the stored status and the
/// current Muscat time (only Confirmed / Cancelled are stored).
/// </summary>
public static class BookingDisplayStatus
{
    public const string Upcoming = "Upcoming";
    public const string InProgress = "InProgress";
    public const string Completed = "Completed";
    public const string Cancelled = "Cancelled";
}