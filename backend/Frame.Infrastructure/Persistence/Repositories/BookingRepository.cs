using Frame.Application.Common.Abstractions.Persistence;
using Frame.Domain.Entities;
using Frame.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Frame.Infrastructure.Persistence.Repositories;

/// <summary>EF Core implementation of IBookingRepository.</summary>
internal sealed class BookingRepository : IBookingRepository
{
    private readonly FrameDbContext _db;

    public BookingRepository(FrameDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Booking>> GetConfirmedForStudioAsync(
        int studioId,
        DateOnly from,
        DateOnly to,
        CancellationToken cancellationToken = default)
        => await _db.Bookings
            .AsNoTracking()
            .Where(b => b.StudioId == studioId
                        && b.BookingDate >= from
                        && b.BookingDate <= to
                        && b.Status == BookingStatus.Confirmed)
            .ToListAsync(cancellationToken);

    public Task<bool> HasOverlapAsync(
        int studioId,
        DateOnly date,
        int startHour,
        int endHour,
        CancellationToken cancellationToken = default)
        // Same rule as Booking.Overlaps, written so EF Core can translate it to SQL (EXISTS).
        => _db.Bookings.AnyAsync(
            b => b.StudioId == studioId
                 && b.BookingDate == date
                 && b.Status == BookingStatus.Confirmed
                 && b.StartHour < endHour
                 && b.EndHour > startHour,
            cancellationToken);

    public async Task<int> NextBookingSequenceAsync(CancellationToken cancellationToken = default)
    {
        // ToListAsync, not SingleAsync: SingleAsync wraps the query in a subquery,
        // and SQL Server does not allow NEXT VALUE FOR inside a subquery.
        var values = await _db.Database
            .SqlQueryRaw<int>("SELECT CAST(NEXT VALUE FOR dbo.BookingNumbers AS int) AS [Value]")
            .ToListAsync(cancellationToken);

        return values.Single();
    }

    public void Add(Booking booking) => _db.Bookings.Add(booking);
}