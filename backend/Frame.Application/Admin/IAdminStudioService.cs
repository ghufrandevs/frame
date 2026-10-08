using Frame.Application.Admin.Dtos;

namespace Frame.Application.Admin;

/// <summary>Studios for the admin panel: list all, add, edit, pause and resume.</summary>
public interface IAdminStudioService
{
    /// <summary>Every studio, paused ones included, with both languages.</summary>
    Task<IReadOnlyList<AdminStudioResponse>> GetAllAsync(CancellationToken cancellationToken);

    Task<AdminStudioResponse> CreateAsync(AdminStudioRequest request, CancellationToken cancellationToken);

    /// <summary>A new price applies to new bookings only: existing invoices keep their own price.</summary>
    Task<AdminStudioResponse> UpdateAsync(int studioId, AdminStudioRequest request, CancellationToken cancellationToken);

    /// <summary>Paused studios disappear for customers; existing bookings stay.</summary>
    Task<AdminStudioResponse> SetStatusAsync(int studioId, bool isActive, CancellationToken cancellationToken);
}