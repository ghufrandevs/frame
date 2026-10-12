namespace Frame.Application.Common.Errors;

/// <summary>
/// Thrown by IUnitOfWork when another request changed the same data at the
/// same moment: a deadlock (two customers booking the same hour) or a stale
/// row (two admins cancelling the same booking). The caller decides the response:
/// for a booking, refund and answer 409 SLOT_TAKEN; for a cancellation, 409
/// ALREADY_CANCELLED. Keeps database-specific errors out of the Application layer.
/// </summary>
public sealed class TransactionConflictException : Exception
{
    public TransactionConflictException(Exception innerException)
        : base("The transaction lost a race with a concurrent transaction.", innerException)
    {
    }
}