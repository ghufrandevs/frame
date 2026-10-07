using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Frame.Application.Common.Errors;

namespace Frame.Api.Extensions;

/// <summary>
/// Reads the logged-in user's id from the validated JWT.
/// The id ALWAYS comes from the token, never from the URL or the body,
/// so a user can never act as someone else by changing a number.
/// </summary>
internal static class ClaimsPrincipalExtensions
{
    public static int GetUserId(this ClaimsPrincipal user)
    {
        // "sub" is mapped to NameIdentifier by the JWT handler; accept both to be safe.
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);

        return int.TryParse(value, out var id) && id > 0
            ? id
            : throw AppException.Unauthorized();
    }
}
