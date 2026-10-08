using System.Globalization;
using Frame.Application.Common.Abstractions;
using Frame.Application.Common.Abstractions.Persistence;
using Frame.Application.Common.Errors;
using Frame.Application.Studios.Dtos;
using Frame.Domain.Entities;

namespace Frame.Application.Studios;

/// <summary>
/// Public studio pages. Loads data through the repositories, applies the
/// scheduling rules from StudioSchedule, and maps entities to DTOs in the
/// request language. No HTTP, no EF Core, no DateTime.Now in here.
/// </summary>
internal sealed class StudioService : IStudioService
{
    private const string MonthFormat = "yyyy-MM";

    private readonly IStudioRepository _studios;
    private readonly IBookingRepository _bookings;
    private readonly IClock _clock;
    private readonly ICurrentLanguage _language;

    public StudioService(
        IStudioRepository studios,
        IBookingRepository bookings,
        IClock clock,
        ICurrentLanguage language)
    {
        _studios = studios;
        _bookings = bookings;
        _clock = clock;
        _language = language;
    }

    public async Task<IReadOnlyList<StudioResponse>> GetActiveStudiosAsync(CancellationToken cancellationToken = default)
    {
        var studios = await _studios.GetActiveAsync(cancellationToken);
        return studios.Select(ToResponse).ToList();
    }

    public async Task<StudioResponse> GetStudioAsync(int studioId, CancellationToken cancellationToken = default)
    {
        var studio = await _studios.GetActiveOrThrowAsync(studioId, cancellationToken);
        return ToResponse(studio);
    }

    public async Task<StudioDaysResponse> GetDaysAsync(int studioId, string month, CancellationToken cancellationToken = default)
    {
        var firstOfMonth = ParseMonth(month);
        var studio = await _studios.GetActiveOrThrowAsync(studioId, cancellationToken);

        var now = _clock.MuscatNow;
        var minDate = StudioSchedule.FirstBookableDay(now);
        var maxDate = StudioSchedule.LastBookableDay(now);
        var lastOfMonth = firstOfMonth.AddMonths(1).AddDays(-1);

        if (lastOfMonth < minDate)
            throw new AppException(ErrorType.Validation, "PAST_TIME");
        if (firstOfMonth > maxDate)
            throw new AppException(ErrorType.Validation, "TOO_FAR_AHEAD");

        // Only the part of the month inside the booking window.
        var from = firstOfMonth > minDate ? firstOfMonth : minDate;
        var to = lastOfMonth < maxDate ? lastOfMonth : maxDate;

        // ONE query for the whole range, then group by day in memory.
        var bookings = await _bookings.GetConfirmedForStudioAsync(studio.Id, from, to, cancellationToken);
        var bookingsByDay = bookings
            .GroupBy(b => b.BookingDate)
            .ToDictionary(g => g.Key, g => (IReadOnlyCollection<Booking>)g.ToList());

        var days = new List<DayAvailabilityDto>();
        for (var day = from; day <= to; day = day.AddDays(1))
        {
            var bookingsOfDay = bookingsByDay.GetValueOrDefault(day, Array.Empty<Booking>());
            days.Add(new DayAvailabilityDto(day, StudioSchedule.HasAvailability(studio, day, bookingsOfDay, now)));
        }

        return new StudioDaysResponse(
            firstOfMonth.ToString(MonthFormat, CultureInfo.InvariantCulture),
            minDate,
            maxDate,
            days);
    }

    public async Task<AvailabilityResponse> GetAvailabilityAsync(int studioId, DateOnly date, CancellationToken cancellationToken = default)
    {
        var now = _clock.MuscatNow;
        StudioSchedule.EnsureBookable(date, now);

        var studio = await _studios.GetActiveOrThrowAsync(studioId, cancellationToken);
        var bookingsOfDay = await _bookings.GetConfirmedForStudioAsync(studio.Id, date, date, cancellationToken);

        return new AvailabilityResponse(
            studio.Id,
            date,
            StudioSchedule.GetSlots(studio, date, bookingsOfDay, now));
    }

    // ===== Helpers =====

    /// <summary>"2026-10" → 2026-10-01. Anything else is a validation error on "month".</summary>
    private static DateOnly ParseMonth(string? month)
    {
        if (!string.IsNullOrWhiteSpace(month)
            && DateOnly.TryParseExact($"{month.Trim()}-01", "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var first))
        {
            return first;
        }

        throw AppException.Validation(new Dictionary<string, string[]>
        {
            ["month"] = [FieldErrorCodes.Required]
        });
    }

    private StudioResponse ToResponse(Studio studio) => new(
        studio.Id,
        studio.LocalizedName(_language),
        studio.LocalizedDescription(_language),
        studio.ImageUrl,
        studio.PricePerHour,
        studio.OpenHour,
        studio.CloseHour);
}