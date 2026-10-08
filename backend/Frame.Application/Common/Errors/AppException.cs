namespace Frame.Application.Common.Errors;

/// <summary>
/// An expected failure that the user should be told about
/// (slot taken, email already used, card declined...).
/// Thrown by services; caught by the API middleware, which turns it into
/// { status, code, message, errors, traceId } with the right HTTP status.
/// Anything that is NOT an AppException (or a DomainException) is treated
/// as a bug: logged in full and answered with a generic 500.
/// </summary>
public sealed class AppException : Exception
{
    public ErrorType Type { get; }

    /// <summary>Stable code from ErrorCodes, translated by the frontend.</summary>
    public string Code { get; }

    /// <summary>
    /// Per-field error codes for validation failures,
    /// e.g. { "email": ["EMAIL_INVALID"] }. Null for other errors.
    /// </summary>
    public IReadOnlyDictionary<string, string[]>? FieldErrors { get; }

    public AppException(ErrorType type, string code, IReadOnlyDictionary<string, string[]>? fieldErrors = null)
        : base(code)
    {
        Type = type;
        Code = code;
        FieldErrors = fieldErrors;
    }

    // ===== Short, readable factories used by the services =====

    public static AppException Validation(IReadOnlyDictionary<string, string[]> fieldErrors)
        => new(ErrorType.Validation, ErrorCodes.ValidationError, fieldErrors);

    public static AppException Unauthorized(string code = ErrorCodes.Unauthorized)
        => new(ErrorType.Unauthorized, code);

    public static AppException PaymentRequired(string code)
        => new(ErrorType.PaymentRequired, code);

    public static AppException Forbidden()
        => new(ErrorType.Forbidden, ErrorCodes.Forbidden);

    public static AppException NotFound(string code = ErrorCodes.NotFound)
        => new(ErrorType.NotFound, code);

    public static AppException Conflict(string code)
        => new(ErrorType.Conflict, code);
}
