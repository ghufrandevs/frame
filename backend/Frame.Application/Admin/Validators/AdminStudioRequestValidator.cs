using FluentValidation;
using Frame.Application.Admin.Dtos;
using Frame.Application.Common.Errors;

namespace Frame.Application.Admin.Validators;

/// <summary>
/// Rules for POST and PUT /api/admin/studios. Max lengths match the column sizes.
/// Prices keep at most 3 decimals (baisa), so the database never rounds silently.
/// </summary>
public sealed class AdminStudioRequestValidator : AbstractValidator<AdminStudioRequest>
{
    private const int MaxNameLength = 100;
    private const int MaxDescriptionLength = 1000;
    private const int MaxImageUrlLength = 500;

    public AdminStudioRequestValidator()
    {
        // Stop at the first failed rule per field: one clear message, not three.
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.NameAr)
            .NotEmpty().WithErrorCode(FieldErrorCodes.Required)
            .MaximumLength(MaxNameLength).WithErrorCode(FieldErrorCodes.TooLong);

        RuleFor(x => x.NameEn)
            .NotEmpty().WithErrorCode(FieldErrorCodes.Required)
            .MaximumLength(MaxNameLength).WithErrorCode(FieldErrorCodes.TooLong);

        RuleFor(x => x.DescriptionAr)
            .NotEmpty().WithErrorCode(FieldErrorCodes.Required)
            .MaximumLength(MaxDescriptionLength).WithErrorCode(FieldErrorCodes.TooLong);

        RuleFor(x => x.DescriptionEn)
            .NotEmpty().WithErrorCode(FieldErrorCodes.Required)
            .MaximumLength(MaxDescriptionLength).WithErrorCode(FieldErrorCodes.TooLong);

        RuleFor(x => x.ImageUrl)
            .NotEmpty().WithErrorCode(FieldErrorCodes.Required)
            .MaximumLength(MaxImageUrlLength).WithErrorCode(FieldErrorCodes.TooLong);

        RuleFor(x => x.PricePerHour)
            .NotNull().WithErrorCode(FieldErrorCodes.Required)
            .GreaterThan(0).WithErrorCode(FieldErrorCodes.PriceInvalid)
            .Must(HaveAtMostThreeDecimals).WithErrorCode(FieldErrorCodes.PriceInvalid);

        RuleFor(x => x.OpenHour)
            .NotNull().WithErrorCode(FieldErrorCodes.Required)
            .InclusiveBetween(0, 23).WithErrorCode(FieldErrorCodes.HoursInvalid);

        RuleFor(x => x.CloseHour)
            .NotNull().WithErrorCode(FieldErrorCodes.Required)
            .InclusiveBetween(1, 24).WithErrorCode(FieldErrorCodes.HoursInvalid)
            .Must((request, closeHour) => CloseAfterOpen(request.OpenHour, closeHour))
                .WithErrorCode(FieldErrorCodes.HoursInvalid);
    }

    private static bool HaveAtMostThreeDecimals(decimal? price)
        => price is null || decimal.Round(price.Value, 3) == price.Value;

    // A missing OpenHour already has its own error, so it is not reported twice here.
    private static bool CloseAfterOpen(int? openHour, int? closeHour)
        => openHour is null || closeHour is null || closeHour > openHour;
}