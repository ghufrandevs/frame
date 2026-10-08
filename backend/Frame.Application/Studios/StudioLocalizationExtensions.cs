using Frame.Application.Common.Abstractions;
using Frame.Domain.Entities;

namespace Frame.Application.Studios;

/// <summary>
/// Picks the studio text in one language, in one place, for every response
/// and email that shows a studio. The entity stores both languages;
/// choosing one is an Application concern.
/// </summary>
internal static class StudioLocalizationExtensions
{
    public static string LocalizedName(this Studio studio, bool isArabic)
        => isArabic ? studio.NameAr : studio.NameEn;

    public static string LocalizedDescription(this Studio studio, bool isArabic)
        => isArabic ? studio.DescriptionAr : studio.DescriptionEn;

    /// <summary>In the language of the current request (Accept-Language).</summary>
    public static string LocalizedName(this Studio studio, ICurrentLanguage language)
        => studio.LocalizedName(language.IsArabic);

    /// <summary>In the language of the current request (Accept-Language).</summary>
    public static string LocalizedDescription(this Studio studio, ICurrentLanguage language)
        => studio.LocalizedDescription(language.IsArabic);
}