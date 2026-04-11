namespace Coucher.Shared.Models;

public sealed class PagedQuery
{
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 50;
}
