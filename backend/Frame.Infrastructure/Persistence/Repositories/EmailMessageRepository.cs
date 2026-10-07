using Frame.Application.Common.Abstractions.Persistence;
using Frame.Domain.Entities;

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
}