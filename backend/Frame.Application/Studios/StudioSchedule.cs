using Frame.Application.Common.Errors;
using Frame.Application.Studios.Dtos;
using Frame.Domain.Entities;

namespace Frame.Application.Studios;

/// <summary>
/// The scheduling rules in one place, as pure functions (no database, no clock):
/// which days can be booked, and which hours of a day are free.
/// Calendar, hours list and (later) booking all use these same rules,
/// so the page can never show as free an hour the server would refuse.
/// </summary>
internal static class StudioSchedule
{
    /// <summary>Today in Muscat: the first day a customer can book.</summary>
    public static DateOnly FirstBookableDay(DateTime nowMuscat)
        => DateOnly.FromDateTime(nowMuscat);

    /// <summary>The last bookable day: one year ahead (same limit Booking.Create enforces).</summary>
    public static DateOnly LastBookableDay(DateTime nowMuscat)
        => FirstBookableDay(nowMuscat).AddMonths(Booking.MaxMonthsAhead);

    /// <summary>Throws PAST_TIME or TOO_FAR_AHEAD when a date is outside the booking window.</summary>
    public static void EnsureBookable(DateOnly date, DateTime nowMuscat)
    {
        if (date < FirstBookableDay(nowMuscat))
            throw new AppException(ErrorType.Validation, "PAST_TIME");

        if (date > LastBookableDay(nowMuscat))
            throw new AppException(ErrorType.Validation, "TOO_FAR_AHEAD");
    }

    /// <summary>
    /// Every hour of the studio's opening time on that date, except hours that
    /// already started. Each hour is Booked if any confirmed booking covers it.
    /// </summary>
    /// <param name="bookingsOfDay">Confirmed bookings of this studio on this date only.</param>
    public static IReadOnlyList<SlotDto> GetSlots(
        Studio studio,
        DateOnly date,
        IReadOnlyCollection<Booking> bookingsOfDay,
        DateTime nowMuscat)
    {
        var slots = new List<SlotDto>();

        for (var hour = studio.OpenHour; hour < studio.CloseHour; hour++)
        {
            var startsAt = date.ToDateTime(new TimeOnly(hour, 0));
            if (startsAt <= nowMuscat)
                continue;

            var isBooked = bookingsOfDay.Any(b => b.Overlaps(hour, hour + 1));
            slots.Add(new SlotDto(hour, isBooked ? SlotStatus.Booked : SlotStatus.Available));
        }

        return slots;
    }

    /// <summary>True if the day still has at least one free hour.</summary>
    public static bool HasAvailability(
        Studio studio,
        DateOnly date,
        IReadOnlyCollection<Booking> bookingsOfDay,
        DateTime nowMuscat)
        => GetSlots(studio, date, bookingsOfDay, nowMuscat).Any(s => s.Status == SlotStatus.Available);
}
