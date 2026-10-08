using Frame.Domain.Entities;

namespace Frame.Application.Common.Abstractions.Persistence;

/// <summary>
/// Data access for studios: active ones for the public pages,
/// all of them for the admin panel, and adding new studios.
/// </summary>
public interface IStudioRepository
{
    /// <summary>Active studios only, ordered by id, for the home page. Read-only (not tracked).</summary>
    Task<IReadOnlyList<Studio>> GetActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>Every studio, paused ones included, ordered by id, for the admin panel. Read-only.</summary>
    Task<IReadOnlyList<Studio>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>One studio, active or not. Tracked, so changes can be saved.</summary>
    Task<Studio?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>Tracks a new studio. Written to the database by IUnitOfWork.</summary>
    void Add(Studio studio);
}