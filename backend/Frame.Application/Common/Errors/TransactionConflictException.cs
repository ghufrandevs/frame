namespace Frame.Application.Common.Errors;

/// <summary>
/// Thrown by IUnitOfWork when the database cancelled a transaction because
/// another one changed the same data at the same moment (e.g. two customers
/// booking the same hour). The caller decides the response: for a booking,
/// refund and answer 409 SLOT_TAKEN. Keeps database-specific errors out of
/// the Application layer.
/// </summary>
public sealed class TransactionConflictException : Exception
{
    public TransactionConflictException(Exception innerException)
        : base("The transaction lost a race with a concurrent transaction.", innerException)
    {
    }
}