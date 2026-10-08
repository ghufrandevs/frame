namespace Frame.Application.Studios.Dtos;

/// <summary>
/// Hours of one studio on one day (GET /api/studios/{id}/availability?date=2026-10-08).
/// Hours that already started are not returned. No customer names, ever.
/// "Selected" is a frontend-only state, never sent by the server.
/// </summary>
public sealed record AvailabilityResponse(
    int StudioId,
    DateOnly Date,
    IReadOnlyList<SlotDto> Slots);

/// <summary>One bookable hour: Hour = 10 means 10:00 to 11:00.</summary>
public sealed record SlotDto(
    int Hour,
    string Status);

/// <summary>The only two values of SlotDto.Status (part of the API contract).</summary>
public static class SlotStatus
{
    public const string Available = "Available";
    public const string Booked = "Booked";
}
