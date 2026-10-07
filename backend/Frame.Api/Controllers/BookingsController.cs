using Frame.Api.Extensions;
using Frame.Api.Security;
using Frame.Application.Bookings;
using Frame.Application.Bookings.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Frame.Api.Controllers;

/// <summary>
/// Customer bookings. Secure by default: every endpoint needs a customer token,
/// except the price quote, which visitors see before they log in.
/// </summary>
[ApiController]
[Route("api/bookings")]
[Authorize(Policy = AuthPolicies.Customer)]
public sealed class BookingsController : ControllerBase
{
    private readonly IBookingService _bookings;

    public BookingsController(IBookingService bookings)
    {
        _bookings = bookings;
    }

    /// <summary>Price summary for a selected time, before payment. Saves nothing.</summary>
    [AllowAnonymous]
    [HttpPost("quote")]
    [ProducesResponseType<QuoteResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<QuoteResponse>> Quote(
        [FromBody] QuoteRequest request,
        CancellationToken cancellationToken)
    {
        var quote = await _bookings.QuoteAsync(request, cancellationToken);
        return Ok(quote);
    }

    /// <summary>
    /// Pays and books. The customer comes from the token, never from the body.
    /// 402: card refused. 409 SLOT_TAKEN: someone else booked that time first.
    /// </summary>
    [HttpPost]
    [ProducesResponseType<BookingResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status402PaymentRequired)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<BookingResponse>> Create(
        [FromBody] CreateBookingRequest request,
        CancellationToken cancellationToken)
    {
        var booking = await _bookings.CreateAsync(User.GetUserId(), request, cancellationToken);
        return Created($"/api/bookings/{booking.Id}", booking);
    }
}