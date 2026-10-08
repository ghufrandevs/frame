using Frame.Domain.Common;
using Frame.Domain.Enums;

namespace Frame.Domain.Entities;

/// <summary>
/// An email waiting to be sent (Outbox pattern).
/// It is saved in the SAME transaction as the booking change,
/// so every booking is guaranteed to get its email, even if the
/// mail server is down or the app restarts. A background service
/// sends Pending messages and records the result here.
/// </summary>
public sealed class EmailMessage : BaseEntity
{
    public const int MaxAttempts = 3;
    public const int MaxErrorLength = 1000;

    public int BookingId { get; private set; }
    public Booking Booking { get; private set; } = null!;

    public EmailType Type { get; private set; }
    public string ToEmail { get; private set; } = null!;

    /// <summary>"ar" or "en": the language the customer used when booking.</summary>
    public string Language { get; private set; } = null!;

    public EmailStatus Status { get; private set; }
    public int Attempts { get; private set; }
    public string? LastError { get; private set; }
    public DateTime? SentAt { get; private set; }

    private EmailMessage() { }

    /// <summary>
    /// Queues an email for a booking. Takes the Booking object (not its Id)
    /// because a new booking has no Id until it is saved; EF links them on insert.
    /// </summary>
    public static EmailMessage Queue(Booking booking, EmailType type, string toEmail, string language)
    {
        var lang = Guard.NotEmpty(language, "EMAIL_LANGUAGE_INVALID").ToLowerInvariant();
        if (lang is not ("ar" or "en"))
            throw new DomainException("EMAIL_LANGUAGE_INVALID");

        return new EmailMessage
        {
            Booking = booking,
            Type = type,
            ToEmail = User.NormalizeEmail(toEmail),
            Language = lang,
            Status = EmailStatus.Pending,
            Attempts = 0
        };
    }

    /// <summary>The mail server accepted the email.</summary>
    public void MarkSent(DateTime nowUtc)
    {
        EnsurePending();
        Attempts++;
        Status = EmailStatus.Sent;
        SentAt = nowUtc;
        LastError = null;
    }

    /// <summary>
    /// Sending failed. Stays Pending for a retry until MaxAttempts,
    /// then becomes Failed and keeps the last error for review.
    /// </summary>
    public void RecordFailure(string error)
    {
        EnsurePending();
        Attempts++;

        var message = string.IsNullOrWhiteSpace(error) ? "Unknown error" : error.Trim();
        LastError = message.Length > MaxErrorLength ? message[..MaxErrorLength] : message;

        if (Attempts >= MaxAttempts)
            Status = EmailStatus.Failed;
    }

    private void EnsurePending()
    {
        if (Status != EmailStatus.Pending)
            throw new DomainException("EMAIL_NOT_PENDING");
    }
}
