namespace Frame.Application.Emails;

/// <summary>
/// Sends the pending emails of the outbox. Public so the background worker
/// (Infrastructure) can call it; the implementation stays internal.
/// </summary>
public interface IEmailOutboxProcessor
{
    /// <summary>Sends one batch of pending emails. Returns how many were processed.</summary>
    Task<int> ProcessPendingAsync(CancellationToken cancellationToken);
}