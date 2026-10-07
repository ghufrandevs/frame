using System.Data;

namespace Frame.Application.Common.Abstractions.Persistence;

/// <summary>
/// Saves all pending changes from every repository as ONE unit.
/// Repositories only collect changes (Add, entity methods);
/// nothing reaches the database until SaveChangesAsync is called.
/// </summary>
public interface IUnitOfWork
{
    /// <summary>Writes every pending change in a single database transaction.</summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Runs several reads and writes inside one explicit transaction.
    /// Used by booking: the clash check and the insert must happen together
    /// (Serializable), so two people can never book the same hour. Payment runs
    /// before it, so the database is never locked while waiting for the provider.
    /// If anything throws, everything is rolled back. If the database cancels the
    /// transaction because a concurrent one touched the same data, this throws
    /// TransactionConflictException.
    /// </summary>
    Task<T> ExecuteInTransactionAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        IsolationLevel isolationLevel,
        CancellationToken cancellationToken = default);
}