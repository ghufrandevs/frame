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
/// (never refund a booking that cannot be cancelled), then the refund, then save.
/// If the refund fails nothing is saved, so the booking stays Confirmed.
/// </summary>
internal sealed class AdminBookingService : IAdminBookingService
{
    private const int PageSize = 20;
    private const string DefaultEmailLanguage = "ar";

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

        // 2. Refund. On failure nothing is saved, so the booking stays Confirmed.
        var refunded = await _paymentGateway.RefundAsync(booking.PaymentReference, booking.TotalAmount, CancellationToken.None);
        if (!refunded)
        {
            _logger.LogError("Cancellation of {BookingNumber} stopped: refund of {Amount} OMR for {Reference} failed",
                booking.BookingNumber, booking.TotalAmount, booking.PaymentReference);
            throw new AppException(ErrorType.Conflict, ErrorCodes.RefundFailed);
        }

        // 3. Email in the language the customer booked in.
        var language = await _emails.GetBookingLanguageAsync(booking.Id, cancellationToken) ?? DefaultEmailLanguage;
        _emails.Add(EmailMessage.Queue(booking, EmailType.BookingCancelled, booking.User.Email, language));

        // 4. The money is back: record it even if the admin closed the page.
        await _unitOfWork.SaveChangesAsync(CancellationToken.None);

        _logger.LogInformation("Booking {BookingNumber} cancelled by admin, {Amount} OMR refunded to {Reference}",
            booking.BookingNumber, booking.TotalAmount, booking.PaymentReference);

        return _mapper.ToAdminResponse(booking);
    }
}