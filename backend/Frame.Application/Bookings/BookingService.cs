using Frame.Application.Bookings.Dtos;
using Frame.Application.Common.Abstractions;
using Frame.Application.Common.Abstractions.Persistence;
using Frame.Application.Studios;

namespace Frame.Application.Bookings;

/// <summary>
/// Customer booking flow. Prices are always computed here from the studio's
/// stored price; the client never sends a price. Time rules come from
/// StudioSchedule so the calendar and the booking can never disagree.
/// </summary>
internal sealed class BookingService : IBookingService
{
    private readonly IStudioRepository _studios;
    private readonly IPriceCalculator _priceCalculator;
    private readonly IClock _clock;
    private readonly ICurrentLanguage _language;

    public BookingService(
        IStudioRepository studios,
        IPriceCalculator priceCalculator,
        IClock clock,
        ICurrentLanguage language)
    {
        _studios = studios;
        _priceCalculator = priceCalculator;
        _clock = clock;
        _language = language;
    }

    public async Task<QuoteResponse> QuoteAsync(QuoteRequest request, CancellationToken cancellationToken)
    {
        // QuoteRequestValidator already rejected missing fields, so these have values.
        var studioId = request.StudioId!.Value;
        var date = request.Date!.Value;
        var startHour = request.StartHour!.Value;
        var endHour = request.EndHour!.Value;

        var studio = await _studios.GetActiveOrThrowAsync(studioId, cancellationToken);
        StudioSchedule.EnsureCanBook(studio, date, startHour, endHour, _clock.MuscatNow);

        var hours = endHour - startHour;
        var price = _priceCalculator.Calculate(studio.PricePerHour, hours);

        return new QuoteResponse(
            studio.Id,
            _language.IsArabic ? studio.NameAr : studio.NameEn,
            date,
            startHour,
            endHour,
            hours,
            price.HourlyRate,
            price.Subtotal,
            _priceCalculator.VatRate,
            price.VatAmount,
            price.TotalAmount);
    }
}