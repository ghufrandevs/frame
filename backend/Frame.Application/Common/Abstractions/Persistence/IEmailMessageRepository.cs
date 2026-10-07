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
}