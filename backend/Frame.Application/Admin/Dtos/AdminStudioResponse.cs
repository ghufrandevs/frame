namespace Frame.Application.Admin.Dtos;

/// <summary>A studio as the admin sees it: both languages, the price, the hours and whether it is active.</summary>
public sealed record AdminStudioResponse(
    int Id,
    string NameAr,
    string NameEn,
    string DescriptionAr,
    string DescriptionEn,
    string ImageUrl,
    decimal PricePerHour,
    int OpenHour,
    int CloseHour,
    bool IsActive);