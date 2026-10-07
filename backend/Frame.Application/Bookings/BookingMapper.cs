using Frame.Application.Bookings.Dtos;
using Frame.Application.Common.Abstractions;
using Frame.Application.Studios;
using Frame.Domain.Entities;
using Frame.Domain.Enums;

namespace Frame.Application.Bookings;

/// <summary>
/// Turns a Booking into what the customer sees, in one place: the full
/// response (create, details) and the short card ("my bookings").
/// Computes the display status from the current Muscat time and shows
/// stored UTC times as Muscat time.
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

    /// <summary>
    /// Full booking with invoice. Studio and customer name are passed in,
    /// so it works right after creation (navigation properties not loaded).
    /// </summary>
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

    /// <summary>Short card for "my bookings". Requires booking.Studio to be loaded (Include).</summary>
    public BookingSummaryResponse ToSummary(Booking booking) => new(
        booking.Id,
        booking.BookingNumber,
        booking.Studio.LocalizedName(_language),
        booking.BookingDate,
        booking.StartHour,
        booking.EndHour,
        booking.TotalAmount,
        DisplayStatus(booking));

    private string DisplayStatus(Booking booking)
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