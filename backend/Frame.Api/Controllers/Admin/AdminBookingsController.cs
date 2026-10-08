using Frame.Application.Admin;
using Frame.Application.Bookings.Dtos;
using Frame.Application.Common.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Frame.Api.Controllers.Admin;

/// <summary>
/// Admin bookings. Protected by AdminControllerBase (admin role + admin-app token + admin CORS).
/// Cancel is POST, not DELETE: the booking is kept, marked Cancelled with its refund.
/// </summary>
[Route("api/admin/bookings")]
public sealed class AdminBookingsController : AdminControllerBase
{
    private readonly IAdminBookingService _bookings;

    public AdminBookingsController(IAdminBookingService bookings)
    {
        _bookings = bookings;
    }

    /// <summary>All bookings, 20 per page, newest first. Optional ?studioId= and ?date=yyyy-MM-dd.</summary>
    [HttpGet]
    [ProducesResponseType<PagedResponse<BookingSummaryResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<PagedResponse<BookingSummaryResponse>>> GetPage(
        [FromQuery] int? studioId,
        [FromQuery] DateOnly? date,
        CancellationToken cancellationToken,
        [FromQuery] int page = 1)
    {
        var result = await _bookings.GetPageAsync(studioId, date, page, cancellationToken);
        return Ok(result);
    }

    /// <summary>One booking with invoice and customer contact details.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType<BookingResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<BookingResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var booking = await _bookings.GetAsync(id, cancellationToken);
        return Ok(booking);
    }

    /// <summary>Cancels before the start with a full refund and emails the customer.</summary>
    [HttpPost("{id:int}/cancel")]
    [ProducesResponseType<BookingResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BookingResponse>> Cancel(int id, CancellationToken cancellationToken)
    {
        var booking = await _bookings.CancelAsync(id, cancellationToken);
        return Ok(booking);
    }
}