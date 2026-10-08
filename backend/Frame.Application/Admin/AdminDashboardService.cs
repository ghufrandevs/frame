using System.Globalization;
using Frame.Application.Admin.Dtos;
using Frame.Application.Bookings;
using Frame.Application.Common.Abstractions;
using Frame.Application.Common.Abstractions.Persistence;
using Frame.Application.Studios;
using Frame.Domain.Enums;

namespace Frame.Application.Admin;

/// <summary>
/// Dashboard rules (from the spec):
/// - Net revenue = paid this month − refunded this month (Muscat month).
/// - Occupancy = confirmed hours in the next 7 days ÷ opening hours of active studios.
/// Stored times are UTC, so the month is checked with IClock.ToMuscat, never a hard-coded offset.
/// </summary>
internal sealed class AdminDashboardService : IAdminDashboardService
{
    private const int OccupancyDays = 7;

    // Wide enough to contain the whole Muscat month; narrowed in memory below.
    private const int MonthLookbackDays = 32;

    private readonly IBookingRepository _bookings;
    private readonly IStudioRepository _studios;
    private readonly BookingMapper _mapper;
    private readonly IClock _clock;
    private readonly ICurrentLanguage _language;

    public AdminDashboardService(
        IBookingRepository bookings,
        IStudioRepository studios,
        BookingMapper mapper,
        IClock clock,
        ICurrentLanguage language)
    {
        _bookings = bookings;
        _studios = studios;
        _mapper = mapper;
        _clock = clock;
        _language = language;
    }

    public async Task<DashboardResponse> GetAsync(CancellationToken cancellationToken)
    {
        var today = _clock.MuscatToday;
        var activeStudios = await _studios.GetActiveAsync(cancellationToken);

        // ===== Money: this Muscat month =====
        var moneyMoves = await _bookings.GetPaidOrCancelledSinceAsync(_clock.UtcNow.AddDays(-MonthLookbackDays), cancellationToken);

        var paidThisMonth = moneyMoves.Where(b => IsThisMonth(b.PaidAt, today)).ToList();
        var refundedThisMonth = moneyMoves
            .Where(b => b.CancelledAt is { } cancelledAt && IsThisMonth(cancelledAt, today))
            .Sum(b => b.RefundAmount ?? 0m);

        var netRevenue = paidThisMonth.Sum(b => b.TotalAmount) - refundedThisMonth;

        var revenueByStudio = activeStudios
            .Select(studio => new StudioRevenueDto(
                studio.Id,
                studio.LocalizedName(_language),
                paidThisMonth
                    .Where(b => b.StudioId == studio.Id && b.Status == BookingStatus.Confirmed)
                    .Sum(b => b.TotalAmount)))
            .OrderByDescending(r => r.Revenue)
            .ToList();

        // ===== Next 7 days (today included): occupancy + today's list, one query =====
        var nextDays = await _bookings.GetConfirmedBetweenAsync(today, today.AddDays(OccupancyDays - 1), cancellationToken);

        var capacityHours = activeStudios.Sum(s => s.CloseHour - s.OpenHour) * OccupancyDays;
        var bookedHours = nextDays.Where(b => b.Studio.IsActive).Sum(b => b.Hours);
        var occupancy = capacityHours == 0 ? 0m : Math.Round((decimal)bookedHours / capacityHours, 2);

        var todayBookings = nextDays
            .Where(b => b.BookingDate == today)
            .Select(_mapper.ToAdminSummary)
            .ToList();

        return new DashboardResponse(
            today.ToString("yyyy-MM", CultureInfo.InvariantCulture),
            netRevenue,
            paidThisMonth.Count,
            occupancy,
            revenueByStudio,
            todayBookings);
    }

    private bool IsThisMonth(DateTime utc, DateOnly today)
    {
        var muscat = _clock.ToMuscat(utc);
        return muscat.Year == today.Year && muscat.Month == today.Month;
    }
}