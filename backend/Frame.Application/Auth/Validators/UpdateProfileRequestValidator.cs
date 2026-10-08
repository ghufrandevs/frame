using FluentValidation;
using Frame.Application.Auth.Dtos;

namespace Frame.Application.Auth.Validators;

/// <summary>Rules for PUT /api/auth/me: exactly the same name and phone rules as sign-up.</summary>
public sealed class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.FullName).ValidFullName();
        RuleFor(x => x.Phone).OmaniMobile();
    }
}