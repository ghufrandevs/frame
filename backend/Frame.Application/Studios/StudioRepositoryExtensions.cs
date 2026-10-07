using Frame.Application.Common.Abstractions.Persistence;
using Frame.Application.Common.Errors;
using Frame.Domain.Entities;

namespace Frame.Application.Studios;

/// <summary>
/// Shared lookups on top of IStudioRepository. The repository only loads data;
/// the "not found" rule and its error code belong to the Application layer.
/// </summary>
internal static class StudioRepositoryExtensions
{
    /// <summary>A paused or missing studio looks the same to customers: not found.</summary>
    public static async Task<Studio> GetActiveOrThrowAsync(
        this IStudioRepository studios,
        int studioId,
        CancellationToken cancellationToken)
    {
        var studio = await studios.GetByIdAsync(studioId, cancellationToken);

        if (studio is null || !studio.IsActive)
            throw AppException.NotFound(ErrorCodes.StudioNotFound);

        return studio;
    }
}