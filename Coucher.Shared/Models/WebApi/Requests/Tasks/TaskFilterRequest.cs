using Coucher.Shared.Models.WebApi.Requests.Common;

namespace Coucher.Shared.Models.WebApi.Requests.Tasks;

public sealed class TaskFilterRequest
{
    public Guid? SeriesId { get; set; }
    public Guid? StatusId { get; set; }
    public bool MineOnly { get; set; }
    public string? SearchTerm { get; set; }
    public required PagedQueryRequest Paging { get; set; }
}
