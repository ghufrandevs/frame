using Frame.Application.Admin;
using Frame.Application.Admin.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Frame.Api.Controllers.Admin;

/// <summary>Admin home page numbers. Protected by AdminControllerBase.</summary>
[Route("api/admin/dashboard")]
public sealed class AdminDashboardController : AdminControllerBase
{
    private readonly IAdminDashboardService _dashboard;

    public AdminDashboardController(IAdminDashboardService dashboard)
    {
        _dashboard = dashboard;
    }

    [HttpGet]
    [ProducesResponseType<DashboardResponse>(StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardResponse>> Get(CancellationToken cancellationToken)
        => Ok(await _dashboard.GetAsync(cancellationToken));
}