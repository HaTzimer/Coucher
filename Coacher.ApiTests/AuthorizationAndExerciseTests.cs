namespace Coacher.ApiTests;

[Collection(ManualApiCollection.Name)]
public sealed class AuthorizationAndExerciseTests
{
    private readonly ManualApiFixture _fixture;

    public AuthorizationAndExerciseTests(ManualApiFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task B1_AdminCanPromoteSeededUserToAuditor()
    {
        var scenario = await _fixture.GetScenarioAsync();

        Assert.NotEqual(Guid.Empty, scenario.AuditorRoleId);
        Assert.NotEqual(Guid.Empty, scenario.AuditorSession.UserId);
    }

    [Fact]
    public async Task B2_NonAdminCannotUpdateUserRole()
    {
        var scenario = await _fixture.GetScenarioAsync();

        using var response = await _fixture.ApiClient.PutJsonAsync(
            $"/api/admin/user-role/update/{scenario.AuditorRoleId}",
            new UpdateUserRoleRequest
            {
                UserId = scenario.AuditorSession.UserId,
                Role = GlobalRole.User
            },
            scenario.User1Session.SessionId
        );

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task E1_RegularUserCanCreateExercise_AndVisibilityIncludesParticipantScope()
    {
        var scenario = await _fixture.GetScenarioAsync();

        using var createResponse = await _fixture.CreateExerciseAsync(
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

        Assert.Equal(HttpStatusCode.OK, createResponse.StatusCode);
        Assert.NotNull(createResponse.JsonBody);

        var exerciseId = createResponse.JsonBody!.RootElement.GetRequiredGuid("id");
        Assert.Equal(scenario.User1Session.UserId, createResponse.JsonBody.RootElement.GetRequiredGuid("createdByUserId"));

        var visibilityQuery = $$"""
            query VisibilityCheck {
              exercises(where: { id: { eq: "{{exerciseId}}" } }) {
                id
                createdByUserId
                participants {
                  id
                  userId
                  role
                }
              }
            }
            """;

        using var creatorVisibility = await _fixture.ApiClient.GraphQlAsync(visibilityQuery, scenario.User1Session.SessionId);
        using var participantVisibility = await _fixture.ApiClient.GraphQlAsync(visibilityQuery, scenario.User2Session.SessionId);
        using var unrelatedVisibility = await _fixture.ApiClient.GraphQlAsync(visibilityQuery, scenario.UnrelatedUserSession.SessionId);

        Assert.Single(creatorVisibility.JsonBody!.RootElement.GetRequiredProperty("data").GetArrayItems("exercises"));
        Assert.Single(participantVisibility.JsonBody!.RootElement.GetRequiredProperty("data").GetArrayItems("exercises"));
        Assert.Empty(unrelatedVisibility.JsonBody!.RootElement.GetRequiredProperty("data").GetArrayItems("exercises"));
    }

    [Fact]
    public async Task E2_AuditorCannotCreateExercise()
    {
        var scenario = await _fixture.GetScenarioAsync();

        using var response = await _fixture.CreateExerciseAsync(scenario.AuditorSession, scenario.AuditorSession.UserId);

        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task E3_AdminCanCreateExercise()
    {
        var scenario = await _fixture.GetScenarioAsync();

        using var response = await _fixture.CreateExerciseAsync(scenario.AdminSession, scenario.AdminSession.UserId);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.JsonBody);
    }

    [Fact]
    public async Task E4_ManagerCanUpdateExercise_ButParticipantCannot()
    {
        var scenario = await _fixture.GetScenarioAsync();

        using var createResponse = await _fixture.CreateExerciseAsync(
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

        var exerciseId = createResponse.JsonBody!.RootElement.GetRequiredGuid("id");

        using var managerUpdateResponse = await _fixture.ApiClient.PutJsonAsync(
            $"/api/exercise/update/{exerciseId}",
            new UpdateExerciseRequest
            {
                Name = "manager-updated-name"
            },
            scenario.User1Session.SessionId
        );

        Assert.Equal(HttpStatusCode.OK, managerUpdateResponse.StatusCode);

        using var participantUpdateResponse = await _fixture.ApiClient.PutJsonAsync(
            $"/api/exercise/update/{exerciseId}",
            new UpdateExerciseRequest
            {
                Name = "participant-should-not-update"
            },
            scenario.User2Session.SessionId
        );

        Assert.Equal(HttpStatusCode.Forbidden, participantUpdateResponse.StatusCode);
    }
}
