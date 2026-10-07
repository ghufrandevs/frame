namespace Frame.Domain.Common;

/// <summary>
/// Small reusable checks that entities use to protect their own state.
/// Each failed check throws a DomainException with a stable error code.
/// Internal: only the Domain project can use it.
/// </summary>
internal static class Guard
{
    /// <summary>
    /// Ensures a text value is not null, empty or whitespace,
    /// and returns it trimmed.
    /// </summary>
    public static string NotEmpty(string? value, string errorCode)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException(errorCode);

        return value.Trim();
    }

    /// <summary>
    /// Ensures a money amount is greater than zero.
    /// </summary>
    public static decimal Positive(decimal value, string errorCode)
    {
        if (value <= 0)
            throw new DomainException(errorCode);

        return value;
    }
}
