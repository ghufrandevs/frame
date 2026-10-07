namespace Frame.Application.Common.Errors;

/// <summary>
/// Stable error codes returned to the frontend, which translates them (ar/en).
/// A code never changes once the frontend uses it; add new ones instead.
/// Business-rule codes thrown by entities (e.g. PAST_TIME, BOOKING_ALREADY_STARTED)
/// live in the Domain and reach the frontend the same way.
/// </summary>
public static class ErrorCodes
{
    // ===== General =====
    public const string ValidationError = "VALIDATION_ERROR";
    public const string Unauthorized = "UNAUTHORIZED";
    public const string Forbidden = "FORBIDDEN";
    public const string NotFound = "NOT_FOUND";
    public const string TooManyRequests = "TOO_MANY_REQUESTS";
    public const string InternalError = "INTERNAL_ERROR";

    // ===== Auth =====
    public const string EmailTaken = "EMAIL_TAKEN";
    public const string InvalidCredentials = "INVALID_CREDENTIALS";

    // ===== Studios =====
    public const string StudioNotFound = "STUDIO_NOT_FOUND";

    // ===== Bookings =====
    public const string BookingNotFound = "BOOKING_NOT_FOUND";
    public const string SlotTaken = "SLOT_TAKEN";

    // ===== Payment =====
    public const string PaymentDeclined = "PAYMENT_DECLINED";
    public const string InsufficientFunds = "INSUFFICIENT_FUNDS";
    public const string PaymentFailed = "PAYMENT_FAILED";
    public const string RefundFailed = "REFUND_FAILED";
}
