using Frame.Application.Common.Abstractions;

namespace Frame.Api.Services;

/// <summary>
/// ICurrentLanguage from the Accept-Language header.
/// Returns "en" only when the header starts with "en" (e.g. "en", "en-US,en;q=0.9");
/// anything else, or no header at all, means "ar" (the contract default).
/// </summary>
internal sealed class HttpCurrentLanguage : ICurrentLanguage
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public HttpCurrentLanguage(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string Code
    {
        get
        {
            var header = _httpContextAccessor.HttpContext?.Request.Headers.AcceptLanguage.ToString();

            return !string.IsNullOrWhiteSpace(header)
                   && header.TrimStart().StartsWith("en", StringComparison.OrdinalIgnoreCase)
                ? "en"
                : "ar";
        }
    }
}
