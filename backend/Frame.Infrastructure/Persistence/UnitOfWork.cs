using System.Data;
using Frame.Application.Common.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Frame.Infrastructure.Persistence;

/// <summary>
/// EF Core implementation of IUnitOfWork.
/// All repositories in one request share the same FrameDbContext (Scoped),
/// so one SaveChanges here writes everything they collected, together.
/// </summary>
internal sealed class UnitOfWork : IUnitOfWork
{
    private readonly FrameDbContext _db;

    public UnitOfWork(FrameDbContext db)
    {
        _db = db;
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => _db.SaveChangesAsync(cancellationToken);

    public async Task<T> ExecuteInTransactionAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        IsolationLevel isolationLevel,
        CancellationToken cancellationToken = default)
    {
        await using var transaction = await _db.Database.BeginTransactionAsync(isolationLevel, cancellationToken);

        try
        {
            var result = await operation(cancellationToken);

            await _db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return result;
        }
        catch
        {
            // Undo everything in SQL Server, and forget the half-done changes in memory.
            await transaction.RollbackAsync(CancellationToken.None);
            _db.ChangeTracker.Clear();
            throw;
        }
    }
}
