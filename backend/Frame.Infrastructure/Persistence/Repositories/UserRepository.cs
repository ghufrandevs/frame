using Frame.Application.Common.Abstractions.Persistence;
using Frame.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Frame.Infrastructure.Persistence.Repositories;

/// <summary>
/// EF Core implementation of IUserRepository.
/// The only place that knows how users are queried in SQL Server.
/// </summary>
internal sealed class UserRepository : IUserRepository
{
    private readonly FrameDbContext _db;

    public UserRepository(FrameDbContext db)
    {
        _db = db;
    }

    public Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        => _db.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);

    public Task<User?> GetByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default)
        => _db.Users.FirstOrDefaultAsync(u => u.Email == normalizedEmail, cancellationToken);

    public Task<bool> EmailExistsAsync(string normalizedEmail, CancellationToken cancellationToken = default)
        => _db.Users.AnyAsync(u => u.Email == normalizedEmail, cancellationToken);

    public void Add(User user)
        => _db.Users.Add(user);
}
