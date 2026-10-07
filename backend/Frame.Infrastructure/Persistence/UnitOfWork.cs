using System.Data;
using Frame.Application.Common.Abstractions.Persistence;
using Frame.Application.Common.Errors;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Frame.Infrastructure.Persistence;

/// <summary>
/// EF Core implementation of IUnitOfWork.
/// All repositories in one request share the same FrameDbContext (Scoped),
/// so one SaveChanges here writes everything they collected, together.
/// </summary>
internal sealed class UnitOfWork : IUnitOfWork
{
    // SQL Server: "Transaction was deadlocked ... and has been chosen as the deadlock victim."
    private const int DeadlockVictim = 1205;

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
        catch (Exception ex)
        {
            // Undo everything in SQL Server, and forget the half-done changes in memory.
            await RollbackQuietlyAsync(transaction);
            _db.ChangeTracker.Clear();

            // Lost a race with a concurrent transaction: tell the caller in Application terms.
            if (IsDeadlock(ex))
                throw new TransactionConflictException(ex);

            throw;
        }
    }

    /// <summary>
    /// A deadlock victim is already rolled back by SQL Server, so rolling back again
    /// throws InvalidOperationException. Ignore only that, to keep the original error.
    /// </summary>
    private static async Task RollbackQuietlyAsync(IDbContextTransaction transaction)
    {
        try
        {
            await transaction.RollbackAsync(CancellationToken.None);
        }
        catch (InvalidOperationException)
        {
            // Already rolled back by the server.
        }
    }

    /// <summary>EF Core may wrap the SqlException (e.g. in DbUpdateException), so check the whole chain.</summary>
    private static bool IsDeadlock(Exception exception)
    {
        for (var current = exception; current is not null; current = current.InnerException)
        {
            if (current is SqlException { Number: DeadlockVictim })
                return true;
        }

        return false;
    }
}