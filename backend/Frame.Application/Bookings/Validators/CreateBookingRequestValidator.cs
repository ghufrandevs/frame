using FluentValidation;
using Frame.Application.Bookings.Dtos;
using Frame.Application.Common.Errors;

namespace Frame.Application.Bookings.Validators;

/// <summary>
/// Rules for POST /api/bookings. Reuses the quote rules for the time fields,
/// so quote and booking can never disagree. The token's format belongs to the
/// payment provider, so only presence and length are checked here.
/// </summary>
public sealed class CreateBookingRequestValidator : AbstractValidator<CreateBookingRequest>
{
    private const int MaxTokenLength = 100;

    public CreateBookingRequestValidator()
    {
        Include(new QuoteRequestValidator());

        // Stop at the first failed rule per field: one clear message, not three.
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.PaymentToken)
            .NotEmpty().WithErrorCode(FieldErrorCodes.Required)
            .MaximumLength(MaxTokenLength).WithErrorCode(FieldErrorCodes.TooLong);
    }
}