using Frame.Application.Common.Abstractions;
using Frame.Domain.Entities;

namespace Frame.Application.Studios;

/// <summary>
/// Picks the studio text in the request language, in one place, for every
/// response that shows a studio (studio pages, quote, bookings, emails).
/// The entity stores both languages; choosing one is an Application concern.
/// </summary>
internal static class StudioLocalizationExtensions
{
    public static string LocalizedName(this Studio studio, ICurrentLanguage language)
        => language.IsArabic ? studio.NameAr : studio.NameEn;

    public static string LocalizedDescription(this Studio studio, ICurrentLanguage language)
        => language.IsArabic ? studio.DescriptionAr : studio.DescriptionEn;
}