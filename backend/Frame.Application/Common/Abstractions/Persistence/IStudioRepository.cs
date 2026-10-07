using Frame.Domain.Entities;

namespace Frame.Application.Common.Abstractions.Persistence;

/// <summary>
/// Data access for studios. Starts with what the public pages need;
/// admin operations are added when the admin feature is built.
/// </summary>
public interface IStudioRepository
{
    /// <summary>Active studios only, ordered by id, for the home page. Read-only (not tracked).</summary>
    Task<IReadOnlyList<Studio>> GetActiveAsync(CancellationToken cancellationToken = default);

    /// <summary>One studio, active or not. Tracked, so changes can be saved.</summary>
    Task<Studio?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
}
