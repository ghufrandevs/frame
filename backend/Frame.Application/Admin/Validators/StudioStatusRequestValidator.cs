using FluentValidation;
using Frame.Application.Admin.Dtos;
using Frame.Application.Common.Errors;

namespace Frame.Application.Admin.Validators;

/// <summary>Rules for PATCH /api/admin/studios/{id}/status.</summary>
public sealed class StudioStatusRequestValidator : AbstractValidator<StudioStatusRequest>
{
    public StudioStatusRequestValidator()
    {
        RuleFor(x => x.IsActive)
            .NotNull().WithErrorCode(FieldErrorCodes.Required);
    }
}