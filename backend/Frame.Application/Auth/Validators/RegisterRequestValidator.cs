using FluentValidation;
using Frame.Application.Auth.Dtos;
using Frame.Application.Common.Errors;

namespace Frame.Application.Auth.Validators;

/// <summary>
/// Rules for POST /api/auth/register. Each failure returns a field code
/// (FieldErrorCodes) that the frontend translates under the input.
/// Max lengths match the column sizes in the database.
/// </summary>
public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    // Omani mobile: 8 digits starting with 7 or 9, optional +968 prefix.
    private const string OmaniMobilePattern = @"^(\+968)?[79]\d{7}$";

    public RegisterRequestValidator()
    {
        // Stop at the first failed rule per field: one clear message, not three.
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.FullName)
            .NotEmpty().WithErrorCode(FieldErrorCodes.Required)
            .Must(name => name.Trim().Length >= 3).WithErrorCode(FieldErrorCodes.NameTooShort)
            .MaximumLength(100).WithErrorCode(FieldErrorCodes.TooLong);

        RuleFor(x => x.Email)
            .NotEmpty().WithErrorCode(FieldErrorCodes.Required)
            .MaximumLength(256).WithErrorCode(FieldErrorCodes.TooLong)
            .EmailAddress().WithErrorCode(FieldErrorCodes.EmailInvalid);

        RuleFor(x => x.Phone)
            .NotEmpty().WithErrorCode(FieldErrorCodes.Required)
            .Matches(OmaniMobilePattern).WithErrorCode(FieldErrorCodes.PhoneInvalid);

        RuleFor(x => x.Password)
            .NotEmpty().WithErrorCode(FieldErrorCodes.Required)
            .MinimumLength(8).WithErrorCode(FieldErrorCodes.PasswordWeak)
            .MaximumLength(100).WithErrorCode(FieldErrorCodes.TooLong)
            .Must(HaveLetterAndDigit).WithErrorCode(FieldErrorCodes.PasswordWeak);
    }

    private static bool HaveLetterAndDigit(string password)
        => password.Any(char.IsLetter) && password.Any(char.IsDigit);
}
