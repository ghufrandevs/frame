using Frame.Domain.Common;
using Frame.Domain.Enums;
using Frame.Domain.ValueObjects;

namespace Frame.Domain.Entities;

/// <summary>
/// A paid, confirmed studio booking for whole hours on one day.
/// Payment and booking happen together, so a Booking always starts Confirmed
/// and carries a frozen copy of the price (the invoice never changes later).
/// All times are Muscat local time.
/// </summary>
public sealed class Booking : BaseEntity
{
    public const int MinHours = 1;
    public const int MaxHours = 8;
    public const int MaxMonthsAhead = 12;

    /// <summary>Public booking number, e.g. "FR-1042".</summary>
    public string BookingNumber { get; private set; } = null!;

    /// <summary>Invoice number, e.g. "INV-2026-1042".</summary>
    public string InvoiceNumber { get; private set; } = null!;

    public int UserId { get; private set; }
    public User User { get; private set; } = null!;

    public int StudioId { get; private set; }
    public Studio Studio { get; private set; } = null!;

    public DateOnly BookingDate { get; private set; }
    public int StartHour { get; private set; }
    public int EndHour { get; private set; }

    // ===== Price snapshot (copied at payment time) =====
    public decimal HourlyRate { get; private set; }
    public decimal Subtotal { get; private set; }
    public int MorningHours { get; private set; }
    public decimal DiscountAmount { get; private set; }
    public decimal VatAmount { get; private set; }
    public decimal TotalAmount { get; private set; }

    // ===== Payment (safe data only) =====
    public string PaymentReference { get; private set; } = null!;
    public string CardBrand { get; private set; } = null!;
    public string CardLast4 { get; private set; } = null!;
    public DateTime PaidAt { get; private set; }

    // ===== Status =====
    public BookingStatus Status { get; private set; }
    public DateTime? CancelledAt { get; private set; }
    public decimal? RefundAmount { get; private set; }

    // ===== Calculated, not stored =====
    public int Hours => EndHour - StartHour;
    public DateTime StartsAt => BookingDate.ToDateTime(new TimeOnly(StartHour, 0));
    public DateTime EndsAt => BookingDate.ToDateTime(new TimeOnly(EndHour, 0));

    private Booking() { }

    /// <summary>
    /// Creates a confirmed booking after a successful payment.
    /// Re-checks every rule here, so no service can save an invalid booking.
    /// </summary>
    /// <param name="sequenceNumber">Unique running number from the database (e.g. 1042).</param>
    /// <param name="nowMuscat">Current Muscat time from IClock (never DateTime.Now).</param>
    public static Booking Create(
        int sequenceNumber,
        int userId,
        Studio studio,
        DateOnly date,
        int startHour,
        int endHour,
        BookingPrice price,
        PaymentInfo payment,
        DateTime nowMuscat)
    {
        if (sequenceNumber <= 0)
            throw new DomainException("BOOKING_NUMBER_INVALID");

        if (userId <= 0)
            throw new DomainException("USER_REQUIRED");

        if (!studio.IsActive)
            throw new DomainException("STUDIO_INACTIVE");

        var hours = endHour - startHour;
        if (hours < MinHours || hours > MaxHours)
            throw new DomainException("BOOKING_DURATION_INVALID");

        if (!studio.IsWithinOpeningHours(startHour, endHour))
            throw new DomainException("OUTSIDE_OPENING_HOURS");

        var startsAt = date.ToDateTime(new TimeOnly(startHour, 0));
        if (startsAt <= nowMuscat)
            throw new DomainException("PAST_TIME");

        if (date > DateOnly.FromDateTime(nowMuscat).AddMonths(MaxMonthsAhead))
            throw new DomainException("TOO_FAR_AHEAD");

        if (price.HourlyRate != studio.PricePerHour || price.Subtotal != price.HourlyRate * hours)
            throw new DomainException("PRICE_TOTAL_MISMATCH");

        if (price.MorningHours > hours)
            throw new DomainException("PRICE_INVALID");

        return new Booking
        {
            BookingNumber = $"FR-{sequenceNumber}",
            InvoiceNumber = $"INV-{nowMuscat.Year}-{sequenceNumber}",
            UserId = userId,
            StudioId = studio.Id,
            BookingDate = date,
            StartHour = startHour,
            EndHour = endHour,

            HourlyRate = price.HourlyRate,
            Subtotal = price.Subtotal,
            MorningHours = price.MorningHours,
            DiscountAmount = price.DiscountAmount,
            VatAmount = price.VatAmount,
            TotalAmount = price.TotalAmount,

            PaymentReference = payment.Reference,
            CardBrand = payment.CardBrand,
            CardLast4 = payment.CardLast4,
            PaidAt = payment.PaidAt,

            Status = BookingStatus.Confirmed
        };
    }

    /// <summary>
    /// The overlap rule: 10-11 and 11-12 do NOT overlap; 10-12 and 11-13 DO.
    /// The repository query uses the same condition in SQL.
    /// </summary>
    public bool Overlaps(int startHour, int endHour)
        => StartHour < endHour && EndHour > startHour;

    /// <summary>
    /// Admin cancellation with a full refund. Only before the booking starts.
    /// Call this BEFORE refunding at the gateway, so a refused cancellation
    /// never sends money back.
    /// </summary>
    public void Cancel(DateTime nowMuscat, DateTime nowUtc)
    {
        if (Status == BookingStatus.Cancelled)
            throw new DomainException("BOOKING_ALREADY_CANCELLED");

        if (nowMuscat >= StartsAt)
            throw new DomainException("BOOKING_ALREADY_STARTED");

        Status = BookingStatus.Cancelled;
        CancelledAt = nowUtc;
        RefundAmount = TotalAmount;
    }
}
