using Frame.Domain.Entities;

namespace Frame.Application.Common.Abstractions.Persistence;

/// <summary>
/// Data access for users. Only the operations the services actually need;
/// no generic "repository of everything".
/// Emails passed in must already be normalized with User.NormalizeEmail.
/// </summary>
public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<User?> GetByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default);

    Task<bool> EmailExistsAsync(string normalizedEmail, CancellationToken cancellationToken = default);

    /// <summary>Marks the user for insert. Nothing is saved until IUnitOfWork.SaveChangesAsync.</summary>
    void Add(User user);
}
