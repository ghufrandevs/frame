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
}