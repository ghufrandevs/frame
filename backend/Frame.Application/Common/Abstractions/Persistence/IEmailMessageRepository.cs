using Frame.Domain.Entities;

namespace Frame.Application.Common.Abstractions.Persistence;

/// <summary>
/// The email outbox. Messages are added in the same transaction as the
/// booking change, then a background service sends them.
/// </summary>
public interface IEmailMessageRepository
{
    /// <summary>Tracks a new pending email. Written to the database by IUnitOfWork.</summary>
    void Add(EmailMessage message);

    /// <summary>
    /// Oldest pending emails with their booking, studio and customer, up to batchSize.
    /// Tracked, because the sender marks each one Sent or Failed.
    /// </summary>
    Task<IReadOnlyList<EmailMessage>> GetPendingAsync(int batchSize, CancellationToken cancellationToken = default);

    /// <summary>
    /// Language of the booking's first email (the confirmation), so later emails
    /// such as the cancellation reach the customer in the same language. Null if none.
    /// </summary>
    Task<string?> GetBookingLanguageAsync(int bookingId, CancellationToken cancellationToken = default);
}