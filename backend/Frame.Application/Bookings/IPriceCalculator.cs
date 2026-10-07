using Frame.Domain.ValueObjects;

namespace Frame.Application.Bookings;

public interface IPriceCalculator
{
    /// <summary>VAT rate applied on the subtotal (0.05 = 5%).</summary>
    decimal VatRate { get; }

    BookingPrice Calculate(decimal hourlyRate, int hours);
}