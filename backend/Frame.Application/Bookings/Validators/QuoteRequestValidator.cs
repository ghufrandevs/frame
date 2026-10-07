using FluentValidation;
using Frame.Application.Bookings.Dtos;
using Frame.Application.Common.Errors;
using Frame.Domain.Entities;

namespace Frame.Application.Bookings.Validators;

/// <summary>
/// Shape rules for POST /api/bookings/quote. Checks that need the database
/// or the clock (studio exists, opening hours, past date, too far ahead)
/// live in the booking service, not here.
/// </summary>
public sealed class QuoteRequestValidator : AbstractValidator<QuoteRequest>
{
    public QuoteRequestValidator()
    {
        // Stop at the first failed rule per field: one clear message, not three.
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.StudioId)
            .NotNull().WithErrorCode(FieldErrorCodes.Required)
            .GreaterThan(0).WithErrorCode(FieldErrorCodes.Required);

        RuleFor(x => x.Date)
            .NotNull().WithErrorCode(FieldErrorCodes.Required);

        RuleFor(x => x.StartHour)
            .NotNull().WithErrorCode(FieldErrorCodes.Required)
            .InclusiveBetween(0, 23).WithErrorCode(FieldErrorCodes.HoursInvalid);

        // EndHour is exclusive: 23:00-24:00 is the last hour of the day.
        RuleFor(x => x.EndHour)
            .NotNull().WithErrorCode(FieldErrorCodes.Required)
            .InclusiveBetween(1, 24).WithErrorCode(FieldErrorCodes.HoursInvalid)
            .Must((request, endHour) => HaveValidLength(request.StartHour, endHour))
                .WithErrorCode(FieldErrorCodes.HoursInvalid);
    }

    // A missing StartHour already has its own error, so it is not reported twice here.
    private static bool HaveValidLength(int? startHour, int? endHour)
    {
        if (startHour is null || endHour is null)
            return true;

        var hours = endHour.Value - startHour.Value;
        return hours >= Booking.MinHours && hours <= Booking.MaxHours;
    }
}