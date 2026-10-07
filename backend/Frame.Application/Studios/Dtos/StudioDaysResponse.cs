namespace Frame.Application.Studios.Dtos;

/// <summary>
/// Calendar data for one studio and one month (GET /api/studios/{id}/days?month=2026-10).
/// Only days between today and the last bookable day are listed.
/// The frontend fades full days and uses MinDate/MaxDate to disable the month arrows.
/// </summary>
public sealed record StudioDaysResponse(
    string Month,
    DateOnly MinDate,
    DateOnly MaxDate,
    IReadOnlyList<DayAvailabilityDto> Days);

/// <summary>One calendar day: does it still have at least one free hour?</summary>
public sealed record DayAvailabilityDto(
    DateOnly Date,
    bool HasAvailability);
