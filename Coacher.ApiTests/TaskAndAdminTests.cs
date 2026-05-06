namespace Coacher.ApiTests;

[Collection(ManualApiCollection.Name)]
public sealed class TaskAndAdminTests
{
    private readonly ManualApiFixture _fixture;

    public TaskAndAdminTests(ManualApiFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task T1_ManagerCanCreateRootTask_AndParticipantCannotCreateRootTask()
    {
        var scenario = await _fixture.GetScenarioAsync();

        using var exerciseResponse = await _fixture.CreateExerciseAsync(
            scenario.User1Session,
            scenario.User1Session.UserId,
            new[]
            {
                new ExerciseParticipantRequest
                {
                    UserId = scenario.User2Session.UserId,
                    Role = ExerciseRole.ExerciseParticipant
                }
            }
        );

        var exerciseId = exerciseResponse.JsonBody!.RootElement.GetRequiredGuid("id");

        using var managerTaskResponse = await _fixture.CreateExerciseTaskAsync(scenario.User1Session, exerciseId);

        Assert.Equal(HttpStatusCode.OK, managerTaskResponse.StatusCode);
        Assert.NotNull(managerTaskResponse.JsonBody);
        Assert.Equal(
            scenario.Catalog.DefaultTaskStatusId,
            managerTaskResponse.JsonBody!.RootElement.GetRequiredGuid("statusId")
        );

        using var participantTaskResponse = await _fixture.CreateExerciseTaskAsync(scenario.User2Session, exerciseId);

        Assert.Equal(HttpStatusCode.Forbidden, participantTaskResponse.StatusCode);
    }

    [Fact]
    public async Task T4_ParticipantCanUpdateTaskStatus_ButCannotEditFullTaskFields()
    {
        var scenario = await _fixture.GetScenarioAsync();

        using var exerciseResponse = await _fixture.CreateExerciseAsync(
            scenario.User1Session,
            scenario.User1Session.UserId,
            new[]
            {
                new ExerciseParticipantRequest
                {
                    UserId = scenario.User2Session.UserId,
                    Role = ExerciseRole.ExerciseParticipant
                }
            }
        );

        var exerciseId = exerciseResponse.JsonBody!.RootElement.GetRequiredGuid("id");

        using var createTaskResponse = await _fixture.CreateExerciseTaskAsync(scenario.User1Session, exerciseId);
        var taskId = createTaskResponse.JsonBody!.RootElement.GetRequiredGuid("id");

        using var allowedUpdateResponse = await _fixture.ApiClient.PutJsonAsync(
            $"/api/exercise-task/update/{taskId}",
            new UpdateExerciseTaskRequest
            {
                StatusId = scenario.Catalog.DefaultTaskStatusId,
                DueDate = DateTime.UtcNow.AddDays(10)
            },
            scenario.User2Session.SessionId
        );

        Assert.Equal(HttpStatusCode.OK, allowedUpdateResponse.StatusCode);

        using var forbiddenUpdateResponse = await _fixture.ApiClient.PutJsonAsync(
            $"/api/exercise-task/update/{taskId}",
            new UpdateExerciseTaskRequest
            {
                Name = "participant-should-not-rename"
            },
            scenario.User2Session.SessionId
        );

        Assert.Equal(HttpStatusCode.Forbidden, forbiddenUpdateResponse.StatusCode);
    }

    [Fact]
    public async Task T6_ManagerCanAddDependencies_ButParticipantCannot()
    {
        var scenario = await _fixture.GetScenarioAsync();

        using var exerciseResponse = await _fixture.CreateExerciseAsync(
            scenario.User1Session,
            scenario.User1Session.UserId,
            new[]
            {
                new ExerciseParticipantRequest
                {
                    UserId = scenario.User2Session.UserId,
                    Role = ExerciseRole.ExerciseParticipant
                }
            }
        );

        var exerciseId = exerciseResponse.JsonBody!.RootElement.GetRequiredGuid("id");

        using var firstTaskResponse = await _fixture.CreateExerciseTaskAsync(scenario.User1Session, exerciseId);
        using var secondTaskResponse = await _fixture.CreateExerciseTaskAsync(scenario.User1Session, exerciseId);

        var firstTaskId = firstTaskResponse.JsonBody!.RootElement.GetRequiredGuid("id");
        var secondTaskId = secondTaskResponse.JsonBody!.RootElement.GetRequiredGuid("id");

        using var managerDependencyResponse = await _fixture.ApiClient.PostJsonAsync(
            $"/api/exercise-task/{secondTaskId}/add-dependencies",
            new List<string> { firstTaskId.ToString() },
            scenario.User1Session.SessionId
        );

        Assert.Equal(HttpStatusCode.OK, managerDependencyResponse.StatusCode);

        using var participantDependencyResponse = await _fixture.ApiClient.PostJsonAsync(
            $"/api/exercise-task/{secondTaskId}/add-dependencies",
            new List<string> { firstTaskId.ToString() },
            scenario.User2Session.SessionId
        );

        Assert.Equal(HttpStatusCode.Forbidden, participantDependencyResponse.StatusCode);
    }

    [Fact]
    public async Task C1_AdminCanCreateClosedListItem_ButRegularUserCannot()
    {
        var scenario = await _fixture.GetScenarioAsync();

        using var adminResponse = await _fixture.CreateClosedListItemAsync(scenario.AdminSession, "AutomationTestKey");
        Assert.Equal(HttpStatusCode.OK, adminResponse.StatusCode);

        using var userResponse = await _fixture.CreateClosedListItemAsync(scenario.User1Session, "AutomationTestKey");
        Assert.Equal(HttpStatusCode.Forbidden, userResponse.StatusCode);
    }

    [Fact]
    public async Task TT1_AdminCanCreateTaskTemplate_ButRegularUserCannot()
    {
        var scenario = await _fixture.GetScenarioAsync();

        using var adminResponse = await _fixture.CreateTaskTemplateAsync(scenario.AdminSession);
        Assert.Equal(HttpStatusCode.OK, adminResponse.StatusCode);

        using var userResponse = await _fixture.CreateTaskTemplateAsync(scenario.User1Session);
        Assert.Equal(HttpStatusCode.Forbidden, userResponse.StatusCode);
    }
}
