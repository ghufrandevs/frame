using System.Data;
using Frame.Application.Bookings.Dtos;
using Frame.Application.Common.Abstractions;
using Frame.Application.Common.Abstractions.Payments;
using Frame.Application.Common.Abstractions.Persistence;
using Frame.Application.Common.Errors;
using Frame.Application.Studios;
using Frame.Domain.Entities;
using Frame.Domain.Enums;
using Frame.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Frame.Application.Bookings;

/// <summary>
/// Customer booking flow. Prices are always computed here from the studio's
/// stored price; the client never sends a price. Time rules come from
/// StudioSchedule so the calendar and the booking can never disagree.
/// Booking: fast clash check, charge, then re-check and insert under a
/// Serializable lock. Any failure after the charge refunds it at once.
/// </summary>
internal sealed class BookingService : IBookingService
{
    private readonly IStudioRepository _studios;
    private readonly IBookingRepository _bookings;
    private readonly IUserRepository _users;
    private readonly IEmailMessageRepository _emails;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentGateway _paymentGateway;
    private readonly IPriceCalculator _priceCalculator;
    private readonly BookingMapper _mapper;
    private readonly IClock _clock;
    private readonly ICurrentLanguage _language;
    private readonly ILogger<BookingService> _logger;

    public BookingService(
        IStudioRepository studios,
        IBookingRepository bookings,
        IUserRepository users,
        IEmailMessageRepository emails,
        IUnitOfWork unitOfWork,
        IPaymentGateway paymentGateway,
        IPriceCalculator priceCalculator,
        BookingMapper mapper,
        IClock clock,
        ICurrentLanguage language,
        ILogger<BookingService> logger)
    {
        _studios = studios;
        _bookings = bookings;
        _users = users;
        _emails = emails;
        _unitOfWork = unitOfWork;
        _paymentGateway = paymentGateway;
        _priceCalculator = priceCalculator;
        _mapper = mapper;
        _clock = clock;
        _language = language;
        _logger = logger;
    }

    public async Task<QuoteResponse> QuoteAsync(QuoteRequest request, CancellationToken cancellationToken)
    {
        var selection = Selection.From(request);
        var (studio, price) = await PriceSelectionAsync(selection, cancellationToken);

        return new QuoteResponse(
            studio.Id,
            studio.LocalizedName(_language),
            selection.Date,
            selection.StartHour,
            selection.EndHour,
            selection.Hours,
            price.HourlyRate,
            price.Subtotal,
            _priceCalculator.VatRate,
            price.VatAmount,
            price.TotalAmount);
    }

    public async Task<BookingResponse> CreateAsync(int userId, CreateBookingRequest request, CancellationToken cancellationToken)
    {
        var selection = Selection.From(request);

        var user = await _users.GetByIdAsync(userId, cancellationToken)
            ?? throw new AppException(ErrorType.Unauthorized, ErrorCodes.Unauthorized);

        var (studio, price) = await PriceSelectionAsync(selection, cancellationToken);

        // 1. Fast check before charging: almost every clash stops here, with nothing charged.
        if (await _bookings.HasOverlapAsync(studio.Id, selection.Date, selection.StartHour, selection.EndHour, cancellationToken))
            throw SlotTaken();

        // 2. Charge the total computed on the server.
        var charge = await _paymentGateway.ChargeAsync(request.PaymentToken!, price.TotalAmount, cancellationToken);
        if (!charge.IsSuccess)
            throw new AppException(ErrorType.PaymentRequired, charge.FailureCode!);

        var payment = new PaymentInfo(charge.Reference!, charge.CardBrand!, charge.CardLast4!, _clock.UtcNow);

        // 3. Re-check and insert under a lock. The customer paid, so ANY failure here is refunded.
        Booking booking;
        try
        {
            booking = await _unitOfWork.ExecuteInTransactionAsync(
                ct => InsertBookingAsync(user, studio, selection, price, payment, ct),
                IsolationLevel.Serializable,
                cancellationToken);
        }
        catch (Exception ex)
        {
            await RefundAsync(payment, price.TotalAmount);

            if (IsSlotLost(ex))
                throw SlotTaken();

            throw;
        }

        _logger.LogInformation("Booking {BookingNumber} created for user {UserId}: studio {StudioId}, {Date} {StartHour}-{EndHour}, {Total} OMR",
            booking.BookingNumber, user.Id, studio.Id, selection.Date, selection.StartHour, selection.EndHour, price.TotalAmount);

        return _mapper.ToResponse(booking, studio, user.FullName);
    }

    public async Task<IReadOnlyList<BookingSummaryResponse>> GetMyBookingsAsync(int userId, CancellationToken cancellationToken)
    {
        var bookings = await _bookings.GetForUserAsync(userId, cancellationToken);
        return bookings.Select(_mapper.ToSummary).ToList();
    }

    public async Task<BookingResponse> GetMyBookingAsync(int userId, int bookingId, CancellationToken cancellationToken)
    {
        var booking = await _bookings.GetByIdWithDetailsAsync(bookingId, cancellationToken);

        // Someone else's booking looks exactly like a missing one: ids cannot be probed.
        if (booking is null || booking.UserId != userId)
            throw AppException.NotFound(ErrorCodes.BookingNotFound);

        return _mapper.ToResponse(booking, booking.Studio, booking.User.FullName);
    }

    // ===== Helpers =====

    /// <summary>Studio, time rules and price: shared by quote and booking, so both always agree.</summary>
    private async Task<(Studio Studio, BookingPrice Price)> PriceSelectionAsync(Selection selection, CancellationToken cancellationToken)
    {
        var studio = await _studios.GetActiveOrThrowAsync(selection.StudioId, cancellationToken);
        StudioSchedule.EnsureCanBook(studio, selection.Date, selection.StartHour, selection.EndHour, _clock.MuscatNow);

        var price = _priceCalculator.Calculate(studio.PricePerHour, selection.Hours);
        return (studio, price);
    }

    /// <summary>Runs inside the Serializable transaction.</summary>
    private async Task<Booking> InsertBookingAsync(
        User user,
        Studio studio,
        Selection selection,
        BookingPrice price,
        PaymentInfo payment,
        CancellationToken cancellationToken)
    {
        // Someone may have booked between the fast check and now; this read also locks the range.
        if (await _bookings.HasOverlapAsync(studio.Id, selection.Date, selection.StartHour, selection.EndHour, cancellationToken))
            throw SlotTaken();

        var sequence = await _bookings.NextBookingSequenceAsync(cancellationToken);

        var booking = Booking.Create(
            sequence, user.Id, studio,
            selection.Date, selection.StartHour, selection.EndHour,
            price, payment, _clock.MuscatNow);

        _bookings.Add(booking);
        _emails.Add(EmailMessage.Queue(booking, EmailType.BookingConfirmed, user.Email, _language.Code));

        return booking;
    }

    /// <summary>
    /// Gives the money back when the booking could not be saved after payment.
    /// Never throws: a failed refund is logged for manual action, and the
    /// original error still reaches the customer.
    /// </summary>
    private async Task RefundAsync(PaymentInfo payment, decimal amount)
    {
        try
        {
            // CancellationToken.None: the money must go back even if the customer closed the page.
            if (await _paymentGateway.RefundAsync(payment.Reference, amount, CancellationToken.None))
            {
                _logger.LogWarning("Booking not saved after payment {Reference}: {Amount} OMR refunded", payment.Reference, amount);
                return;
            }

            _logger.LogError("Booking not saved after payment {Reference} and the refund was refused: refund {Amount} OMR manually",
                payment.Reference, amount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Booking not saved after payment {Reference} and the refund failed: refund {Amount} OMR manually",
                payment.Reference, amount);
        }
    }

    private static bool IsSlotLost(Exception ex)
        => ex is TransactionConflictException
           || ex is AppException { Code: ErrorCodes.SlotTaken };

    private static AppException SlotTaken()
        => new(ErrorType.Conflict, ErrorCodes.SlotTaken);

    /// <summary>The selected time, read once from the request (the validator guaranteed every field).</summary>
    private readonly record struct Selection(int StudioId, DateOnly Date, int StartHour, int EndHour)
    {
        public int Hours => EndHour - StartHour;

        public static Selection From(QuoteRequest request) => new(
            request.StudioId!.Value,
            request.Date!.Value,
            request.StartHour!.Value,
            request.EndHour!.Value);
    }
}