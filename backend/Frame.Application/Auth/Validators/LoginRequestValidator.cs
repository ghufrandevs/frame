using FluentValidation;
using Frame.Application.Auth.Dtos;
using Frame.Application.Common.Errors;

namespace Frame.Application.Auth.Validators;

/// <summary>
/// Rules for both login endpoints. Only checks that the fields are present
/// and not absurdly long: a wrong email or password is answered later with
/// one generic INVALID_CREDENTIALS, never with a hint about which one was wrong.
/// </summary>
public sealed class LoginRequestValidator : AbstractValidator<LoginRequest>
{
    public LoginRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Email)
            .NotEmpty().WithErrorCode(FieldErrorCodes.Required)
            .MaximumLength(256).WithErrorCode(FieldErrorCodes.TooLong);

        RuleFor(x => x.Password)
            .NotEmpty().WithErrorCode(FieldErrorCodes.Required)
            .MaximumLength(100).WithErrorCode(FieldErrorCodes.TooLong);
    }
}
