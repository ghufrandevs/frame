using Frame.Application.Bookings.Dtos;

namespace Frame.Application.Admin.Dtos;

/// <summary>
/// GET /api/admin/dashboard. Money is for the current Muscat month.
/// OccupancyNext7Days is 0..1 (0.42 = 42%). Today reuses the admin booking row.
/// </summary>
public sealed record DashboardResponse(
    string Month,
    decimal NetRevenue,
    int BookingsCount,
    decimal OccupancyNext7Days,
    IReadOnlyList<StudioRevenueDto> RevenueByStudio,
    IReadOnlyList<BookingSummaryResponse> Today);

/// <summary>This month's revenue of one active studio (0 included, so a quiet studio is visible).</summary>
public sealed record StudioRevenueDto(int StudioId, string Name, decimal Revenue);