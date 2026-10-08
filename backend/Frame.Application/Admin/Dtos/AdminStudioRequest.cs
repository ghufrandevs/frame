namespace Frame.Application.Admin.Dtos;

/// <summary>
/// Body of POST and PUT /api/admin/studios. Both languages are edited together.
/// Nullable so a missing field is REQUIRED, not a silent default (0, empty).
/// </summary>
public sealed record AdminStudioRequest
{
    public string? NameAr { get; init; }
    public string? NameEn { get; init; }
    public string? DescriptionAr { get; init; }
    public string? DescriptionEn { get; init; }
    public string? ImageUrl { get; init; }
    public decimal? PricePerHour { get; init; }
    public int? OpenHour { get; init; }
    public int? CloseHour { get; init; }
}