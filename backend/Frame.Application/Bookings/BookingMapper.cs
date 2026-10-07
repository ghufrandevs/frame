using Frame.Application.Bookings.Dtos;
using Frame.Application.Common.Abstractions;
using Frame.Application.Studios;
using Frame.Domain.Entities;
using Frame.Domain.Enums;

namespace Frame.Application.Bookings;

/// <summary>
/// Turns a Booking into the response the customer sees, in one place,
/// for create, "my bookings" and booking details. Computes the display status
/// from the current Muscat time and shows stored UTC times as Muscat time.
/// Studio and customer name are passed in explicitly, so it never depends on
/// navigation properties being loaded.
/// </summary>
internal sealed class BookingMapper
{
    private readonly IClock _clock;
    private readonly ICurrentLanguage _language;

    public BookingMapper(IClock clock, ICurrentLanguage language)
    {
        _clock = clock;
        _language = language;
    }

    public BookingResponse ToResponse(Booking booking, Studio studio, string customerName) => new(
        booking.Id,
        booking.BookingNumber,
        DisplayStatus(booking),
        new BookingStudioDto(studio.Id, studio.LocalizedName(_language), studio.ImageUrl),
        booking.BookingDate,
        booking.StartHour,
        booking.EndHour,
        new InvoiceDto(
            booking.InvoiceNumber,
            _clock.ToMuscat(booking.CreatedAt),
            customerName,
            booking.Hours,
            booking.HourlyRate,
            booking.Subtotal,
            booking.VatAmount,
            booking.TotalAmount),
        new PaymentDto(
            booking.PaymentReference,
            booking.CardBrand,
            booking.CardLast4,
            _clock.ToMuscat(booking.PaidAt)),
        Cancellation(booking));

    public string DisplayStatus(Booking booking)
    {
        if (booking.Status == BookingStatus.Cancelled)
            return BookingDisplayStatus.Cancelled;

        var now = _clock.MuscatNow;

        if (now < booking.StartsAt)
            return BookingDisplayStatus.Upcoming;

        return now < booking.EndsAt
            ? BookingDisplayStatus.InProgress
            : BookingDisplayStatus.Completed;
    }

    private CancellationDto? Cancellation(Booking booking)
        => booking.CancelledAt is { } cancelledAt && booking.RefundAmount is { } refundAmount
            ? new CancellationDto(_clock.ToMuscat(cancelledAt), refundAmount)
            : null;
}