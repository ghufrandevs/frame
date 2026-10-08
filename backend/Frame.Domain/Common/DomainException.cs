namespace Frame.Domain.Common;

/// <summary>
/// Thrown by an entity when an operation would break a business rule,
/// e.g. cancelling a booking that already started.
/// Carries a stable error code that the frontend translates (ar/en).
/// </summary>
public sealed class DomainException : Exception
{
    /// <summary>
    /// Machine-readable error code, e.g. "BOOKING_ALREADY_STARTED".
    /// </summary>
    public string Code { get; }

    public DomainException(string code)
        : base(code)
    {
        Code = code;
    }
}
