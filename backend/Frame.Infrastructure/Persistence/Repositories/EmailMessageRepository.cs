using Frame.Application.Common.Abstractions.Persistence;
using Frame.Domain.Entities;
using Frame.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Frame.Infrastructure.Persistence.Repositories;

/// <summary>EF Core implementation of IEmailMessageRepository.</summary>
internal sealed class EmailMessageRepository : IEmailMessageRepository
{
    private readonly FrameDbContext _db;

    public EmailMessageRepository(FrameDbContext db)
    {
        _db = db;
    }

    public void Add(EmailMessage message) => _db.EmailMessages.Add(message);

    public async Task<IReadOnlyList<EmailMessage>> GetPendingAsync(int batchSize, CancellationToken cancellationToken = default)
        => await _db.EmailMessages
            .Include(m => m.Booking).ThenInclude(b => b.Studio)
            .Include(m => m.Booking).ThenInclude(b => b.User)
            .Where(m => m.Status == EmailStatus.Pending)
            .OrderBy(m => m.Id)
            .Take(batchSize)
            .ToListAsync(cancellationToken);
}