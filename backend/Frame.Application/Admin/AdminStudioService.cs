using Frame.Application.Admin.Dtos;
using Frame.Application.Common.Abstractions.Persistence;
using Frame.Application.Common.Errors;
using Frame.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Frame.Application.Admin;

/// <summary>
/// Admin side of studios. Every change goes through the Studio entity's own
/// methods (Create, UpdateDetails, ChangePrice, ChangeOpeningHours, Activate,
/// Deactivate), so the Domain keeps its rules. The validator already guaranteed
/// every field, so values are read with ! / .Value.
/// </summary>
internal sealed class AdminStudioService : IAdminStudioService
{
    private readonly IStudioRepository _studios;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<AdminStudioService> _logger;

    public AdminStudioService(IStudioRepository studios, IUnitOfWork unitOfWork, ILogger<AdminStudioService> logger)
    {
        _studios = studios;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<IReadOnlyList<AdminStudioResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        var studios = await _studios.GetAllAsync(cancellationToken);
        return studios.Select(ToResponse).ToList();
    }

    public async Task<AdminStudioResponse> CreateAsync(AdminStudioRequest request, CancellationToken cancellationToken)
    {
        var studio = Studio.Create(
            request.NameAr!.Trim(), request.NameEn!.Trim(),
            request.DescriptionAr!.Trim(), request.DescriptionEn!.Trim(),
            request.ImageUrl!.Trim(), request.PricePerHour!.Value,
            request.OpenHour!.Value, request.CloseHour!.Value);

        _studios.Add(studio);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Studio {StudioId} created by admin", studio.Id);
        return ToResponse(studio);
    }

    public async Task<AdminStudioResponse> UpdateAsync(int studioId, AdminStudioRequest request, CancellationToken cancellationToken)
    {
        var studio = await GetOrThrowAsync(studioId, cancellationToken);

        studio.UpdateDetails(
            request.NameAr!.Trim(), request.NameEn!.Trim(),
            request.DescriptionAr!.Trim(), request.DescriptionEn!.Trim(),
            request.ImageUrl!.Trim());
        studio.ChangePrice(request.PricePerHour!.Value);
        studio.ChangeOpeningHours(request.OpenHour!.Value, request.CloseHour!.Value);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Studio {StudioId} updated by admin", studio.Id);
        return ToResponse(studio);
    }

    public async Task<AdminStudioResponse> SetStatusAsync(int studioId, bool isActive, CancellationToken cancellationToken)
    {
        var studio = await GetOrThrowAsync(studioId, cancellationToken);

        if (isActive)
            studio.Activate();
        else
            studio.Deactivate();

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Studio {StudioId} {Status} by admin", studio.Id, isActive ? "activated" : "paused");
        return ToResponse(studio);
    }

    // Unlike the public side, the admin can see and edit paused studios.
    private async Task<Studio> GetOrThrowAsync(int studioId, CancellationToken cancellationToken)
        => await _studios.GetByIdAsync(studioId, cancellationToken)
           ?? throw AppException.NotFound(ErrorCodes.StudioNotFound);

    private static AdminStudioResponse ToResponse(Studio studio) => new(
        studio.Id,
        studio.NameAr,
        studio.NameEn,
        studio.DescriptionAr,
        studio.DescriptionEn,
        studio.ImageUrl,
        studio.PricePerHour,
        studio.OpenHour,
        studio.CloseHour,
        studio.IsActive);
}