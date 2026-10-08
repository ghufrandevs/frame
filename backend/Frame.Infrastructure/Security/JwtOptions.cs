using Microsoft.Extensions.Configuration;

namespace Frame.Infrastructure.Security;

/// <summary>
/// JWT settings, read from configuration section "Jwt".
/// Used by BOTH sides: the generator that signs tokens and the API
/// that validates them, so the two can never disagree.
/// The signing key is a secret: user-secrets locally, .env in Docker.
/// </summary>
public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    /// <summary>Audience for customer tokens: valid on customer routes only.</summary>
    public const string CustomerAudience = "frame-web";

    /// <summary>Audience for admin tokens: valid on /api/admin routes only.</summary>
    public const string AdminAudience = "frame-admin";

    private const int MinKeyLength = 32;

    public string Issuer { get; private init; } = "frame-api";
    public string Key { get; private init; } = string.Empty;
    public int ExpiryHours { get; private init; } = 8;

    /// <summary>Reads and checks the settings; stops the app at startup if the key is missing or too short.</summary>
    public static JwtOptions FromConfiguration(IConfiguration configuration)
    {
        var section = configuration.GetSection(SectionName);
        var key = section["Key"];

        if (string.IsNullOrWhiteSpace(key) || key.Length < MinKeyLength)
            throw new InvalidOperationException(
                $"Setting 'Jwt:Key' is missing or shorter than {MinKeyLength} characters. " +
                "Set it with user-secrets (local) or the Jwt__Key environment variable (Docker).");

        return new JwtOptions
        {
            Key = key,
            Issuer = section["Issuer"] ?? "frame-api",
            ExpiryHours = int.TryParse(section["ExpiryHours"], out var hours) && hours > 0 ? hours : 8
        };
    }
}
