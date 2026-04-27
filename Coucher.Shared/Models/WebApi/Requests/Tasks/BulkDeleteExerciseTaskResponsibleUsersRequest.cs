namespace Coucher.Shared.Models.WebApi.Requests.Tasks;

public sealed class BulkDeleteExerciseTaskResponsibleUsersRequest
{
    public required List<Guid> ResponsibilityIds { get; set; }
}
