namespace Coucher.Shared.Models.WebApi.Requests.Common;

public sealed class PagedQueryRequest
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
