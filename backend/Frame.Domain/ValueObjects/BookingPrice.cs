using Frame.Domain.Common;

namespace Frame.Domain.ValueObjects;

/// <summary>
/// The price breakdown calculated by the server at payment time:
/// Subtotal = hours x hourly rate + optional photographer fee, then VAT on top.
/// Validates that the numbers are consistent before a booking can use them,
/// so an invoice can never show a total that does not add up.
/// All amounts are OMR with 3 decimal places.
/// </summary>
public sealed record BookingPrice
{
    /// <summary>Studio price per hour at the moment of payment (the snapshot).</summary>
    public decimal HourlyRate { get; }

    /// <summary>Optional photographer for the whole booking, 0 when not chosen. Included in Subtotal.</summary>
    public decimal PhotographerFee { get; }

    /// <summary>Hours x HourlyRate + PhotographerFee, before VAT.</summary>
    public decimal Subtotal { get; }

    public decimal VatAmount { get; }

    /// <summary>What the customer actually paid: Subtotal + VatAmount.</summary>
    public decimal TotalAmount { get; }

    public BookingPrice(
        decimal hourlyRate,
        decimal photographerFee,
        decimal subtotal,
        decimal vatAmount,
        decimal totalAmount)
    {
        Guard.Positive(hourlyRate, "PRICE_INVALID");
        Guard.Positive(subtotal, "PRICE_INVALID");

        if (photographerFee < 0 || vatAmount < 0)
            throw new DomainException("PRICE_INVALID");

        if (totalAmount != subtotal + vatAmount)
            throw new DomainException("PRICE_TOTAL_MISMATCH");

        HourlyRate = hourlyRate;
        PhotographerFee = photographerFee;
        Subtotal = subtotal;
        VatAmount = vatAmount;
        TotalAmount = totalAmount;
    }
}