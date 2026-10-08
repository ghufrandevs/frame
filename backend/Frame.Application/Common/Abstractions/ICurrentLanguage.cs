namespace Frame.Application.Common.Abstractions;

/// <summary>
/// The language of the current request: "ar" or "en".
/// Services use it to pick NameAr/NameEn and the email language.
/// The API layer fills it from the Accept-Language header (default "ar").
/// </summary>
public interface ICurrentLanguage
{
    /// <summary>Always "ar" or "en", never anything else.</summary>
    string Code { get; }

    bool IsArabic => Code == "ar";
}
