using FluentValidation;
using Frame.Application.Auth.Dtos;
using Frame.Application.Common.Errors;

namespace Frame.Application.Auth.Validators;

/// <summary>
/// Rules for POST /api/auth/register. Each failure returns a field code
/// (FieldErrorCodes) that the frontend translates under the input.
/// Max lengths match the column sizes in the database.
/// Name and phone rules are shared with the profile page (UserFieldRules).
/// </summary>
public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        // Stop at the first failed rule per field: one clear message, not three.
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.FullName).ValidFullName();

        RuleFor(x => x.Email)
            .NotEmpty().WithErrorCode(FieldErrorCodes.Required)
            .MaximumLength(256).WithErrorCode(FieldErrorCodes.TooLong)
            .EmailAddress().WithErrorCode(FieldErrorCodes.EmailInvalid);

        RuleFor(x => x.Phone).OmaniMobile();

        RuleFor(x => x.Password)
            .NotEmpty().WithErrorCode(FieldErrorCodes.Required)
            .MinimumLength(8).WithErrorCode(FieldErrorCodes.PasswordWeak)
            .MaximumLength(100).WithErrorCode(FieldErrorCodes.TooLong)
            .Must(HaveLetterAndDigit).WithErrorCode(FieldErrorCodes.PasswordWeak);
    }

    private static bool HaveLetterAndDigit(string password)
        => password.Any(char.IsLetter) && password.Any(char.IsDigit);
}