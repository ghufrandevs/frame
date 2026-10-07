namespace Frame.Application.Studios.Dtos;

/// <summary>
/// A studio as the customer sees it (GET /api/studios and /api/studios/{id}).
/// Name and description are already in the request language:
/// the frontend never chooses between Arabic and English fields.
/// </summary>
public sealed record StudioResponse(
    int Id,
    string Name,
    string Description,
    string ImageUrl,
    decimal PricePerHour,
    int OpenHour,
    int CloseHour);
