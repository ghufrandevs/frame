using Frame.Domain.ValueObjects;

namespace Frame.Application.Bookings;

/// <summary>
/// Subtotal = hours x studio rate + (hours x photographer rate, if chosen).
/// VAT applies to the whole subtotal. Amounts rounded to 3 decimals (baisa).
/// </summary>
internal sealed class PriceCalculator : IPriceCalculator
{
    private const decimal Rate = 0.05m;
    private const decimal PhotographerRate = 30.000m;
    private const int Decimals = 3;

    public decimal VatRate => Rate;

    public decimal PhotographerRatePerHour => PhotographerRate;

    public BookingPrice Calculate(decimal hourlyRate, int hours, bool withPhotographer)
    {
        var studioAmount = Round(hourlyRate * hours);
        var photographerFee = withPhotographer ? Round(PhotographerRate * hours) : 0.000m;

        var subtotal = studioAmount + photographerFee;
        var vatAmount = Round(subtotal * VatRate);
        var totalAmount = subtotal + vatAmount;

        return new BookingPrice(hourlyRate, photographerFee, subtotal, vatAmount, totalAmount);
    }

    private static decimal Round(decimal value) =>
        Math.Round(value, Decimals, MidpointRounding.AwayFromZero);
}