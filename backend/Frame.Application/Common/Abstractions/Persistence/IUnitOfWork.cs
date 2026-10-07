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
    /// Used by booking: the clash check, the payment and the insert must happen
    /// together (Serializable), so two people can never book the same hour.
    /// If anything throws, everything is rolled back.
    /// </summary>
    Task<T> ExecuteInTransactionAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        IsolationLevel isolationLevel,
        CancellationToken cancellationToken = default);
}
