namespace Frame.Application.Common.Errors;

/// <summary>
/// Per-field validation codes returned inside "errors",
/// e.g. { "email": ["EMAIL_INVALID"] }. The frontend shows each
/// translated message under its input. Part of the API contract.
/// </summary>
public static class FieldErrorCodes
{
    public const string Required = "REQUIRED";
    public const string TooLong = "TOO_LONG";

    public const string EmailInvalid = "EMAIL_INVALID";
    public const string PhoneInvalid = "PHONE_INVALID";
    public const string PasswordWeak = "PASSWORD_WEAK";
    public const string NameTooShort = "NAME_TOO_SHORT";

    public const string PriceInvalid = "PRICE_INVALID";
    public const string HoursInvalid = "HOURS_INVALID";
}
