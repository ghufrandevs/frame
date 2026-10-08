namespace Frame.Application.Common.Dtos;

/// <summary>
/// One page of any list: { items, page, pageSize, totalCount, totalPages }.
/// TotalCount counts every match, so the UI can show "page 2 of 7".
/// </summary>
public sealed record PagedResponse<T>(IReadOnlyList<T> Items, int Page, int PageSize, int TotalCount)
{
    public int TotalPages => TotalCount == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
}