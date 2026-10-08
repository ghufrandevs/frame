using Frame.Application.Studios;
using Frame.Application.Studios.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Frame.Api.Controllers;

/// <summary>
/// Public studio pages: the list, one studio, the calendar and the hours.
/// Open to visitors: browsing never needs a login.
/// </summary>
[ApiController]
[Route("api/studios")]
[AllowAnonymous]
public sealed class StudiosController : ControllerBase
{
    private readonly IStudioService _studioService;

    public StudiosController(IStudioService studioService)
    {
        _studioService = studioService;
    }

    /// <summary>All active studios, for the home page cards.</summary>
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<StudioResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        => Ok(await _studioService.GetActiveStudiosAsync(cancellationToken));

    /// <summary>One active studio, for the studio page.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType<StudioResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
        => Ok(await _studioService.GetStudioAsync(id, cancellationToken));

    /// <summary>Calendar of one month, e.g. ?month=2026-10.</summary>
    [HttpGet("{id:int}/days")]
    [ProducesResponseType<StudioDaysResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDays(int id, [FromQuery] string month, CancellationToken cancellationToken)
        => Ok(await _studioService.GetDaysAsync(id, month, cancellationToken));

    /// <summary>Free and booked hours of one day, e.g. ?date=2026-10-08.</summary>
    [HttpGet("{id:int}/availability")]
    [ProducesResponseType<AvailabilityResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAvailability(
        int id,
        [FromQuery, BindRequired] DateOnly date,
        CancellationToken cancellationToken)
        => Ok(await _studioService.GetAvailabilityAsync(id, date, cancellationToken));
}
