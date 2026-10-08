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

    public async Task<IReadOnlyList<Booking>> GetForUserAsync(int userId, CancellationToken cancellationToken = default)
        => await _db.Bookings
            .AsNoTracking()
            .Include(b => b.Studio)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.BookingDate)
            .ThenByDescending(b => b.StartHour)
            .ToListAsync(cancellationToken);

    public Task<Booking?> GetByIdWithDetailsAsync(int bookingId, CancellationToken cancellationToken = default)
        => _db.Bookings
            .AsNoTracking()
            .Include(b => b.Studio)
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == bookingId, cancellationToken);

    public async Task<(IReadOnlyList<Booking> Items, int TotalCount)> GetAdminPageAsync(
        int? studioId,
        DateOnly? date,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Bookings.AsNoTracking();

        if (studioId is not null)
            query = query.Where(b => b.StudioId == studioId);

        if (date is not null)
            query = query.Where(b => b.BookingDate == date);

        // Count after the filters, before paging: "page 2 of 7".
        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Include(b => b.Studio)
            .Include(b => b.User)
            .OrderByDescending(b => b.BookingDate)
            .ThenByDescending(b => b.StartHour)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public Task<Booking?> GetByIdForUpdateAsync(int bookingId, CancellationToken cancellationToken = default)
        => _db.Bookings
            .Include(b => b.Studio)
            .Include(b => b.User)
            .FirstOrDefaultAsync(b => b.Id == bookingId, cancellationToken);

    public async Task<IReadOnlyList<Booking>> GetConfirmedBetweenAsync(DateOnly from, DateOnly to, CancellationToken cancellationToken = default)
        => await _db.Bookings
            .AsNoTracking()
            .Include(b => b.Studio)
            .Include(b => b.User)
            .Where(b => b.Status == BookingStatus.Confirmed
                        && b.BookingDate >= from
                        && b.BookingDate <= to)
            .OrderBy(b => b.BookingDate)
            .ThenBy(b => b.StartHour)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Booking>> GetPaidOrCancelledSinceAsync(DateTime sinceUtc, CancellationToken cancellationToken = default)
        => await _db.Bookings
            .AsNoTracking()
            .Include(b => b.Studio)
            .Where(b => b.PaidAt >= sinceUtc
                        || (b.CancelledAt != null && b.CancelledAt >= sinceUtc))
            .ToListAsync(cancellationToken);

    public void Add(Booking booking) => _db.Bookings.Add(booking);
}