namespace Coacher.ApiTests;

[Collection(ManualApiCollection.Name)]
public sealed class GraphQlTests
{
    private readonly ManualApiFixture _fixture;

    public GraphQlTests(ManualApiFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task GS1_AnonymousIntrospection_ReturnsCoacherQuery()
    {
        using var response = await _fixture.ApiClient.GraphQlAsync("query { __schema { queryType { name } } }");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.JsonBody);
        Assert.Equal(
            "CoacherQuery",
            response.JsonBody!.RootElement
                .GetRequiredProperty("data")
                .GetRequiredProperty("__schema")
                .GetRequiredProperty("queryType")
                .GetRequiredString("name")
        );
    }

    [Fact]
    public async Task GS2_AnonymousReadableFields_ReturnDataAndNoVisibleExercises()
    {
        const string query = """
            query AnonymousRead {
              closedListItems {
                id
                value
              }
              exercises {
                id
                name
              }
            }
            """;

        using var response = await _fixture.ApiClient.GraphQlAsync(query);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.JsonBody);

        var data = response.JsonBody!.RootElement.GetRequiredProperty("data");
        Assert.NotEmpty(data.GetArrayItems("closedListItems"));
        Assert.Empty(data.GetArrayItems("exercises"));
    }

    [Fact]
    public async Task GS3_AnonymousAdminOnlyFields_ReturnAuthorizationError()
    {
        using var response = await _fixture.ApiClient.GraphQlAsync("query { users { id } }");

        Assert.Contains(response.StatusCode, new[] { HttpStatusCode.OK, HttpStatusCode.InternalServerError });
        Assert.NotNull(response.JsonBody);

        var errors = response.JsonBody!.RootElement.GetArrayItems("errors");
        Assert.NotEmpty(errors);
        Assert.Equal(
            "AUTHORIZATION_FORBIDDEN",
            errors[0].GetRequiredProperty("extensions").GetRequiredString("code")
        );
    }

    [Fact]
    public async Task G6_RegularUser_AdminOnlyGraphQlFields_AreForbidden()
    {
        var scenario = await _fixture.GetScenarioAsync();

        using var response = await _fixture.ApiClient.GraphQlAsync("query { taskTemplates { id name } }", scenario.User1Session.SessionId);

        Assert.Contains(response.StatusCode, new[] { HttpStatusCode.OK, HttpStatusCode.InternalServerError });
        Assert.NotNull(response.JsonBody);

        var errors = response.JsonBody!.RootElement.GetArrayItems("errors");
        Assert.NotEmpty(errors);
        Assert.Equal(
            "AUTHORIZATION_FORBIDDEN",
            errors[0].GetRequiredProperty("extensions").GetRequiredString("code")
        );
    }

    [Fact]
    public async Task G7_UserNotifications_ReturnOnlyCurrentUsersNotifications()
    {
        var scenario = await _fixture.GetScenarioAsync();

        const string query = """
            query MyNotifications {
              userNotifications {
                id
                userId
                title
                exerciseId
                taskId
              }
            }
            """;

        using var response = await _fixture.ApiClient.GraphQlAsync(query, scenario.User1Session.SessionId);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.JsonBody);

        var notifications = response.JsonBody!.RootElement
            .GetRequiredProperty("data")
            .GetArrayItems("userNotifications");

        Assert.NotEmpty(notifications);
        Assert.All(notifications, notification => Assert.Equal(scenario.User1Session.UserId, notification.GetRequiredGuid("userId")));
    }
}
