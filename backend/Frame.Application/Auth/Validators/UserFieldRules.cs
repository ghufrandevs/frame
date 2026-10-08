using FluentValidation;
using Frame.Application.Common.Errors;

namespace Frame.Application.Auth.Validators;

/// <summary>
/// Name and phone rules shared by sign-up and the profile page,
/// so both can never accept different values.
/// </summary>
internal static class UserFieldRules
{
    public const int MinNameLength = 3;
    public const int MaxNameLength = 100;

    // Omani mobile: 8 digits starting with 7 or 9, optional +968 prefix.
    private const string OmaniMobilePattern = @"^(\+968)?[79]\d{7}$";

    public static IRuleBuilderOptions<T, string> ValidFullName<T>(this IRuleBuilder<T, string> rule)
        => rule
            .NotEmpty().WithErrorCode(FieldErrorCodes.Required)
            .Must(name => name.Trim().Length >= MinNameLength).WithErrorCode(FieldErrorCodes.NameTooShort)
            .MaximumLength(MaxNameLength).WithErrorCode(FieldErrorCodes.TooLong);

    public static IRuleBuilderOptions<T, string> OmaniMobile<T>(this IRuleBuilder<T, string> rule)
        => rule
            .NotEmpty().WithErrorCode(FieldErrorCodes.Required)
            .Matches(OmaniMobilePattern).WithErrorCode(FieldErrorCodes.PhoneInvalid);
}