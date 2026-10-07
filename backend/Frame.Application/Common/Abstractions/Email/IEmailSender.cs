namespace Frame.Application.Common.Abstractions.Email;

/// <summary>
/// Delivers one email. Mailpit (SMTP) locally and in Docker; a real provider
/// later replaces only the implementation. Throws when delivery fails, so
/// the outbox can record the error and retry.
/// </summary>
public interface IEmailSender
{
    Task SendAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken);
}