using Frame.Application.Common.Errors;
using Frame.Domain.Common;
using Microsoft.AspNetCore.WebUtilities;

namespace Frame.Api.Middleware;

/// <summary>
/// Catches every exception that escapes a request and turns it into the
/// contract error shape: { status, code, message, errors, traceId }.
/// Expected errors keep their code; anything unexpected is logged in full
/// and answered with a generic 500, so internal details never leak and
/// one failing request never takes the server down.
/// </summary>
internal sealed class ExceptionMiddleware
{
    // Domain rule codes the user can fix by changing the request → 400.
    private static readonly HashSet<string> BadRequestDomainCodes =
        ["INVALID_TIME_RANGE", "PAST_TIME", "TOO_FAR_AHEAD"];

    // Domain rule codes that clash with the current state → 409.
    private static readonly HashSet<string> ConflictDomainCodes =
        ["STUDIO_INACTIVE", "BOOKING_STARTED", "ALREADY_CANCELLED"];

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (AppException ex)
        {
            _logger.LogInformation("Request refused with {Code}", ex.Code);
            await WriteErrorAsync(context, ToStatusCode(ex.Type), ex.Code, ex.FieldErrors);
        }
        catch (DomainException ex) when (BadRequestDomainCodes.Contains(ex.Code))
        {
            _logger.LogInformation("Business rule refused with {Code}", ex.Code);
            await WriteErrorAsync(context, StatusCodes.Status400BadRequest, ex.Code);
        }
        catch (DomainException ex) when (ConflictDomainCodes.Contains(ex.Code))
        {
            _logger.LogInformation("Business rule refused with {Code}", ex.Code);
            await WriteErrorAsync(context, StatusCodes.Status409Conflict, ex.Code);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            // The client closed the page: nothing to answer, nothing broken.
            _logger.LogDebug("Request cancelled by the client");
        }
        catch (Exception ex)
        {
            // A bug, or a domain rule that validation should have caught first.
            _logger.LogError(ex, "Unhandled exception");
            await WriteErrorAsync(context, StatusCodes.Status500InternalServerError, ErrorCodes.InternalError);
        }
    }

    private static int ToStatusCode(ErrorType type) => type switch
    {
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.PaymentRequired => StatusCodes.Status402PaymentRequired,
        ErrorType.Forbidden => StatusCodes.Status403Forbidden,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        _ => StatusCodes.Status500InternalServerError
    };

    /// <summary>The one place that writes the error JSON. Public so auth events can reuse the same shape.</summary>
    public static async Task WriteErrorAsync(
        HttpContext context,
        int statusCode,
        string code,
        IReadOnlyDictionary<string, string[]>? fieldErrors = null)
    {
        if (context.Response.HasStarted)
            return;

        context.Response.Clear();
        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsJsonAsync(new
        {
            status = statusCode,
            code,
            message = ReasonPhrases.GetReasonPhrase(statusCode),
            errors = fieldErrors,
            traceId = context.TraceIdentifier
        });
    }
}
