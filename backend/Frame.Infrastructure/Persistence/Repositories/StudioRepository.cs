using Frame.Application.Common.Abstractions.Persistence;
using Frame.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Frame.Infrastructure.Persistence.Repositories;

/// <summary>EF Core implementation of IStudioRepository.</summary>
internal sealed class StudioRepository : IStudioRepository
{
    private readonly FrameDbContext _db;

    public StudioRepository(FrameDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Studio>> GetActiveAsync(CancellationToken cancellationToken = default)
        => await _db.Studios
            .AsNoTracking()
            .Where(s => s.IsActive)
            .OrderBy(s => s.Id)
            .ToListAsync(cancellationToken);

    public Task<Studio?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => _db.Studios.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
}
