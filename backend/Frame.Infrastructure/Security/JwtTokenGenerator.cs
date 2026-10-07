using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Frame.Application.Common.Abstractions;
using Frame.Domain.Entities;
using Frame.Domain.Enums;
using Microsoft.IdentityModel.Tokens;

namespace Frame.Infrastructure.Security;

/// <summary>
/// Creates signed JWTs (HMAC-SHA256). The audience follows the user's role,
/// so a customer token can never open admin routes and vice versa.
/// </summary>
internal sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    private static readonly TimeSpan MuscatOffset = TimeSpan.FromHours(4);

    private readonly JwtOptions _options;
    private readonly IClock _clock;
    private readonly SigningCredentials _signingCredentials;
    private readonly JwtSecurityTokenHandler _tokenHandler = new();

    public JwtTokenGenerator(JwtOptions options, IClock clock)
    {
        _options = options;
        _clock = clock;

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.Key));
        _signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
    }

    public AccessToken Generate(User user)
    {
        var now = _clock.UtcNow;
        var expiresAt = now.AddHours(_options.ExpiryHours);

        var audience = user.Role == UserRole.Admin
            ? JwtOptions.AdminAudience
            : JwtOptions.CustomerAudience;

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: audience,
            claims: claims,
            notBefore: now,
            expires: expiresAt,
            signingCredentials: _signingCredentials);

        var value = _tokenHandler.WriteToken(token);

        // The contract shows times with the Muscat offset, e.g. "2026-10-07T20:00:00+04:00".
        return new AccessToken(value, new DateTimeOffset(expiresAt).ToOffset(MuscatOffset));
    }
}
