using Coucher.Shared.Models.Enums;
using Coucher.WebApi.Models.WebApi.Requests.Common;
using TaskProgressStatus = Coucher.Shared.Models.Enums.TaskStatus;

namespace Coucher.WebApi.Models.WebApi.Requests.Tasks;

public sealed class TaskFilterRequestModel
{
    public ExerciseSeries? Series { get; set; }
    public TaskProgressStatus? Status { get; set; }
    public bool MineOnly { get; set; }
    public string? SearchTerm { get; set; }
    public required PagedQueryRequestModel Paging { get; set; }
}
