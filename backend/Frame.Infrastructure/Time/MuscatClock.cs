using Frame.Application.Common.Abstractions;

namespace Frame.Infrastructure.Time;

/// <summary>
/// IClock for Oman. Muscat is always UTC+4 (Oman has no daylight saving),
/// so a fixed offset is exact and works the same on Windows, Linux and
/// inside Docker, without depending on the server's time zone settings.
/// </summary>
internal sealed class MuscatClock : IClock
{
    private static readonly TimeSpan MuscatOffset = TimeSpan.FromHours(4);

    public DateTime UtcNow => DateTime.UtcNow;

    public DateTime MuscatNow => DateTime.SpecifyKind(UtcNow + MuscatOffset, DateTimeKind.Unspecified);

    public DateOnly MuscatToday => DateOnly.FromDateTime(MuscatNow);

    // EF Core reads DateTime back as Unspecified; marking it Utc keeps the result
    // the same on any machine, whatever the server's own time zone is.
    public DateTimeOffset ToMuscat(DateTime utc)
        => new DateTimeOffset(DateTime.SpecifyKind(utc, DateTimeKind.Utc)).ToOffset(MuscatOffset);
}