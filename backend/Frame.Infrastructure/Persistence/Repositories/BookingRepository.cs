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
}
