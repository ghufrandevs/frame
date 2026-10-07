using Frame.Domain.ValueObjects;

namespace Frame.Application.Bookings;

internal sealed class PriceCalculator : IPriceCalculator
{
    private const decimal Rate = 0.05m;
    private const int Decimals = 3;

    public decimal VatRate => Rate;

    public BookingPrice Calculate(decimal hourlyRate, int hours)
    {
        var subtotal = Round(hourlyRate * hours);
        var vatAmount = Round(subtotal * VatRate);
        var totalAmount = subtotal + vatAmount;

        return new BookingPrice(hourlyRate, subtotal, vatAmount, totalAmount);
    }

    private static decimal Round(decimal value) =>
        Math.Round(value, Decimals, MidpointRounding.AwayFromZero);
}