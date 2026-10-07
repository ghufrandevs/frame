using Frame.Application.Common.Abstractions;
using Frame.Application.Common.Abstractions.Email;
using Frame.Application.Common.Abstractions.Persistence;
using Microsoft.Extensions.Logging;

namespace Frame.Application.Emails;

/// <summary>
/// One pass over the outbox: compose, send, mark Sent or record the failure.
/// Saves after EACH email, so a crash can never resend a whole batch.
/// One failing email never blocks the others. Logs the booking number,
/// never the customer's email address.
/// </summary>
internal sealed class EmailOutboxProcessor : IEmailOutboxProcessor
{
    private const int BatchSize = 20;

    private readonly IEmailMessageRepository _emails;
    private readonly IEmailSender _sender;
    private readonly BookingEmailComposer _composer;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IClock _clock;
    private readonly ILogger<EmailOutboxProcessor> _logger;

    public EmailOutboxProcessor(
        IEmailMessageRepository emails,
        IEmailSender sender,
        BookingEmailComposer composer,
        IUnitOfWork unitOfWork,
        IClock clock,
        ILogger<EmailOutboxProcessor> logger)
    {
        _emails = emails;
        _sender = sender;
        _composer = composer;
        _unitOfWork = unitOfWork;
        _clock = clock;
        _logger = logger;
    }

    public async Task<int> ProcessPendingAsync(CancellationToken cancellationToken)
    {
        var messages = await _emails.GetPendingAsync(BatchSize, cancellationToken);

        foreach (var message in messages)
        {
            try
            {
                var content = _composer.Compose(message);
                await _sender.SendAsync(message.ToEmail, content.Subject, content.HtmlBody, cancellationToken);

                message.MarkSent(_clock.UtcNow);
                _logger.LogInformation("Email {Type} for booking {BookingNumber} sent",
                    message.Type, message.Booking.BookingNumber);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                // App is stopping: leave the email Pending, do not burn an attempt.
                throw;
            }
            catch (Exception ex)
            {
                message.RecordFailure(ex.Message);
                _logger.LogWarning(ex, "Email {Type} for booking {BookingNumber} failed (attempt {Attempt}, now {Status})",
                    message.Type, message.Booking.BookingNumber, message.Attempts, message.Status);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return messages.Count;
    }
}