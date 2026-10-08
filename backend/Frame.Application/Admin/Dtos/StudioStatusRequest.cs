namespace Frame.Application.Admin.Dtos;

/// <summary>
/// Body of PATCH /api/admin/studios/{id}/status: { "isActive": false }.
/// Nullable on purpose: a forgotten field must be REQUIRED, never pause a studio by default.
/// </summary>
public sealed record StudioStatusRequest
{
    public bool? IsActive { get; init; }
}