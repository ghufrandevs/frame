using Frame.Application.Admin;
using Frame.Application.Admin.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Frame.Api.Controllers.Admin;

/// <summary>Admin studios. Protected by AdminControllerBase (admin role + admin-app token + admin CORS).</summary>
[Route("api/admin/studios")]
public sealed class AdminStudiosController : AdminControllerBase
{
    private readonly IAdminStudioService _studios;

    public AdminStudiosController(IAdminStudioService studios)
    {
        _studios = studios;
    }

    /// <summary>Every studio, paused ones included, with both languages.</summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<AdminStudioResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<AdminStudioResponse>>> GetAll(CancellationToken cancellationToken)
        => Ok(await _studios.GetAllAsync(cancellationToken));

    [HttpPost]
    [ProducesResponseType<AdminStudioResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AdminStudioResponse>> Create(
        [FromBody] AdminStudioRequest request,
        CancellationToken cancellationToken)
    {
        var studio = await _studios.CreateAsync(request, cancellationToken);
        return Created($"/api/studios/{studio.Id}", studio);
    }

    /// <summary>Edits details, price and hours. A new price applies to new bookings only.</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType<AdminStudioResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdminStudioResponse>> Update(
        int id,
        [FromBody] AdminStudioRequest request,
        CancellationToken cancellationToken)
        => Ok(await _studios.UpdateAsync(id, request, cancellationToken));

    /// <summary>Pauses or resumes a studio: { "isActive": false }.</summary>
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType<AdminStudioResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AdminStudioResponse>> SetStatus(
        int id,
        [FromBody] StudioStatusRequest request,
        CancellationToken cancellationToken)
        => Ok(await _studios.SetStatusAsync(id, request.IsActive!.Value, cancellationToken));
}