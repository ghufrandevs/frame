using Frame.Application.Admin.Dtos;

namespace Frame.Application.Admin;

/// <summary>Numbers for the admin home page.</summary>
public interface IAdminDashboardService
{
    Task<DashboardResponse> GetAsync(CancellationToken cancellationToken);
}