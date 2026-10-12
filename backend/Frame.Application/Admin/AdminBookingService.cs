using Frame.Application.Bookings;
using Frame.Application.Bookings.Dtos;
using Frame.Application.Common.Abstractions;
using Frame.Application.Common.Abstractions.Payments;
using Frame.Application.Common.Abstractions.Persistence;
using Frame.Application.Common.Dtos;
using Frame.Application.Common.Errors;
using Frame.Domain.Entities;
using Frame.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace Frame.Application.Admin;

/// <summary>
/// Admin side of bookings. Cancellation order matters: the Domain validates first
/// (never refund a booking that cannot be cancelled), the cancellation is saved
/// (only one admin can win it), then the refund. If the refund fails the booking
/// is put back to Confirmed, so the customer keeps the session they paid for.
/// </summary>
internal sealed class AdminBookingService : IAdminBookingService
{
    private const int PageSize = 20;
    private const string DefaultEmailLanguage = "ar";

    // Same code Booking.Cancel uses, so the API answers the same either way.
    private const string AlreadyCancelled = "ALREADY_CANCELLED";

    private readonly IBookingRepository _bookings;
    private readonly IEmailMessageRepository _emails;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPaymentGateway _paymentGateway;
    private readonly BookingMapper _mapper;
    private readonly IClock _clock;
    private readonly ILogger<AdminBookingService> _logger;

    public AdminBookingService(
        IBookingRepository bookings,
        IEmailMessageRepository emails,
        IUnitOfWork unitOfWork,
        IPaymentGateway paymentGateway,
        BookingMapper mapper,
        IClock clock,
        ILogger<AdminBookingService> logger)
    {
        _bookings = bookings;
        _emails = emails;
        _unitOfWork = unitOfWork;
        _paymentGateway = paymentGateway;
        _mapper = mapper;
        _clock = clock;
        _logger = logger;
    }

    public async Task<PagedResponse<BookingSummaryResponse>> GetPageAsync(
        int? studioId,
        DateOnly? date,
        int page,
        CancellationToken cancellationToken)
    {
        var safePage = Math.Max(page, 1);
        var (items, totalCount) = await _bookings.GetAdminPageAsync(studioId, date, safePage, PageSize, cancellationToken);

        return new PagedResponse<BookingSummaryResponse>(
            items.Select(_mapper.ToAdminSummary).ToList(),
            safePage,
            PageSize,
            totalCount);
    }

    public async Task<BookingResponse> GetAsync(int bookingId, CancellationToken cancellationToken)
    {
        var booking = await _bookings.GetByIdWithDetailsAsync(bookingId, cancellationToken)
            ?? throw AppException.NotFound(ErrorCodes.BookingNotFound);

        return _mapper.ToAdminResponse(booking);
    }

    public async Task<BookingResponse> CancelAsync(int bookingId, CancellationToken cancellationToken)
    {
        var booking = await _bookings.GetByIdForUpdateAsync(bookingId, cancellationToken)
            ?? throw AppException.NotFound(ErrorCodes.BookingNotFound);

        // 1. Domain rules first: ALREADY_CANCELLED / BOOKING_STARTED stop here, before any money moves.
        booking.Cancel(_clock.MuscatNow, _clock.UtcNow);

        // 2. Claim the cancellation BEFORE refunding. Status is a concurrency token, so if two
        //    admins cancel at the same moment only one save succeeds and only that one refunds.
        //    From here on CancellationToken.None: once money starts moving, finish the job.
        try
        {
            await _unitOfWork.SaveChangesAsync(CancellationToken.None);
        }
        catch (TransactionConflictException)
        {
            _logger.LogInformation("Cancellation of {BookingNumber} refused: another admin cancelled it first", booking.BookingNumber);
            throw new AppException(ErrorType.Conflict, AlreadyCancelled);
        }

        // 3. Refund. If it fails, the booking goes back to Confirmed: nothing changed for the customer.
        if (!await TryRefundAsync(booking))
        {
            booking.RevertCancellation();
            await _unitOfWork.SaveChangesAsync(CancellationToken.None);
            throw new AppException(ErrorType.Conflict, ErrorCodes.RefundFailed);
        }

        // 4. Email in the language the customer booked in.
        var language = await _emails.GetBookingLanguageAsync(booking.Id, CancellationToken.None) ?? DefaultEmailLanguage;
        _emails.Add(EmailMessage.Queue(booking, EmailType.BookingCancelled, booking.User.Email, language));
        await _unitOfWork.SaveChangesAsync(CancellationToken.None);

        _logger.LogInformation("Booking {BookingNumber} cancelled by admin, {Amount} OMR refunded to {Reference}",
            booking.BookingNumber, booking.TotalAmount, booking.PaymentReference);

        return _mapper.ToAdminResponse(booking);
    }

    /// <summary>Full refund at the gateway. Never throws: any failure is logged and returns false.</summary>
    private async Task<bool> TryRefundAsync(Booking booking)
    {
        try
        {
            if (await _paymentGateway.RefundAsync(booking.PaymentReference, booking.TotalAmount, CancellationToken.None))
                return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Refund call for {Reference} threw", booking.PaymentReference);
        }

        _logger.LogError("Cancellation of {BookingNumber} stopped: refund of {Amount} OMR for {Reference} failed",
            booking.BookingNumber, booking.TotalAmount, booking.PaymentReference);
        return false;
    }
}