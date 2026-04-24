using Coucher.WebApi.Models.WebApi.Requests.Common;

namespace Coucher.WebApi.Models.WebApi.Requests.Tasks;

public sealed class TaskFilterRequestModel
{
    public Guid? SeriesClosedListItemId { get; set; }
    public Guid? StatusClosedListItemId { get; set; }
    public bool MineOnly { get; set; }
    public string? SearchTerm { get; set; }
    public required PagedQueryRequestModel Paging { get; set; }
}
