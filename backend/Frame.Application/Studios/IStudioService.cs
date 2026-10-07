using Frame.Application.Studios.Dtos;

namespace Frame.Application.Studios;

/// <summary>
/// Public studio pages: the list, one studio, the calendar and the hours.
/// Everything here is open to visitors (no login). Inactive studios are hidden.
/// </summary>
public interface IStudioService
{
    /// <summary>All active studios, in the request language.</summary>
    Task<IReadOnlyList<StudioResponse>> GetActiveStudiosAsync(CancellationToken cancellationToken = default);

    /// <summary>One active studio. Throws STUDIO_NOT_FOUND if missing or paused.</summary>
    Task<StudioResponse> GetStudioAsync(int studioId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Calendar for one month, "yyyy-MM" (e.g. "2026-10").
    /// Throws PAST_TIME / TOO_FAR_AHEAD for a month outside the booking window.
    /// </summary>
    Task<StudioDaysResponse> GetDaysAsync(int studioId, string month, CancellationToken cancellationToken = default);

    /// <summary>
    /// Free and booked hours of one day.
    /// Throws PAST_TIME / TOO_FAR_AHEAD for a date outside the booking window.
    /// </summary>
    Task<AvailabilityResponse> GetAvailabilityAsync(int studioId, DateOnly date, CancellationToken cancellationToken = default);
}
