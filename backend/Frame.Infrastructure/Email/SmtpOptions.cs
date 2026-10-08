using Microsoft.Extensions.Configuration;

namespace Frame.Infrastructure.Email;

/// <summary>
/// SMTP settings from the "Smtp" section. Defaults point to Mailpit on this
/// machine, so local runs need no setup. Docker sets Smtp__Host=frame-mail.
/// Username/Password only for a real server: user-secrets or .env, never in code.
/// </summary>
public sealed class SmtpOptions
{
    public string Host { get; init; } = "localhost";
    public int Port { get; init; } = 1025;
    public bool UseStartTls { get; init; }
    public string? Username { get; init; }
    public string? Password { get; init; }
    public string FromEmail { get; init; } = "noreply@frame.om";
    public string FromName { get; init; } = "Frame";

    public static SmtpOptions FromConfiguration(IConfiguration configuration)
    {
        var options = new SmtpOptions();
        configuration.GetSection("Smtp").Bind(options);
        return options;
    }
}