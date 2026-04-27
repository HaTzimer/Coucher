namespace Coucher.Shared.Models.WebApi.Requests.Tasks;

public sealed class BulkUpdateExerciseTaskResponsibleUsersRequest
{
    public required List<string> UserIds { get; set; }
}
