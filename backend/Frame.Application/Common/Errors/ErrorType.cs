namespace Frame.Application.Common.Errors;

/// <summary>
/// The kind of failure, independent of HTTP.
/// Application decides WHAT went wrong; the API layer alone
/// decides which HTTP status code that becomes.
/// </summary>
public enum ErrorType
{
    /// <summary>The request data is invalid. → 400</summary>
    Validation = 1,

    /// <summary>Not logged in, or wrong email/password. → 401</summary>
    Unauthorized = 2,

    /// <summary>The payment gateway refused the charge. → 402</summary>
    PaymentRequired = 3,

    /// <summary>Logged in, but not allowed to do this. → 403</summary>
    Forbidden = 4,

    /// <summary>The item does not exist (or is hidden from this user). → 404</summary>
    NotFound = 5,

    /// <summary>The request clashes with the current state, e.g. slot taken. → 409</summary>
    Conflict = 6
}
