namespace Coacher.ApiTests;

[Collection(ManualApiCollection.Name)]
public sealed class AuthenticationTests
{
    private readonly ManualApiFixture _fixture;

    public AuthenticationTests(ManualApiFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task SeedSetup_ReturnsExpectedSummary()
    {
        var scenario = await _fixture.GetScenarioAsync();
        var seed = scenario.SeedResponse;

        Assert.True(seed.GetRequiredProperty("userCount").GetInt32() >= 12);
        Assert.True(seed.GetRequiredProperty("exerciseCount").GetInt32() >= 3);
        Assert.True(seed.GetRequiredProperty("taskTemplateCount").GetInt32() >= 12);
        Assert.True(seed.GetRequiredProperty("externalIdCount").GetInt32() >= 1);
    }

    [Fact]
    public async Task A1_A2_ValidCredentials_ReturnSessionsForSeededUsers()
    {
        var scenario = await _fixture.GetScenarioAsync();

        Assert.False(string.IsNullOrWhiteSpace(scenario.AdminSession.SessionId));
        Assert.False(string.IsNullOrWhiteSpace(scenario.User1Session.SessionId));
        Assert.False(string.IsNullOrWhiteSpace(scenario.User2Session.SessionId));
        Assert.NotEqual(Guid.Empty, scenario.AdminSession.UserId);
        Assert.NotEqual(Guid.Empty, scenario.User1Session.UserId);
        Assert.NotEqual(Guid.Empty, scenario.User2Session.UserId);
    }

    [Fact]
    public async Task A3_WrongCredentials_ReturnUnauthorized()
    {
        using var response = await _fixture.ApiClient.PostJsonAsync("/api/auth/login", new LoginRequest
        {
            IdentityNumber = "300000000",
            PasswordOrPersonalNumber = "wrong-password"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task A4_A6_A7_SessionAndLogoutFlow_WorksForFreshLogin()
    {
        _ = await _fixture.GetScenarioAsync();

        using var loginResponse = await _fixture.ApiClient.PostJsonAsync("/api/auth/login", new LoginRequest
        {
            IdentityNumber = "300000005",
            PasswordOrPersonalNumber = "PN-1005"
        });

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        Assert.NotNull(loginResponse.JsonBody);

        var session = JsonSerializer.Deserialize<AuthenticatedSession>(loginResponse.Body, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.NotNull(session);

        using var sessionResponse = await _fixture.ApiClient.GetAsync("/api/auth/session", session!.SessionId);
        Assert.Equal(HttpStatusCode.OK, sessionResponse.StatusCode);
        Assert.NotNull(sessionResponse.JsonBody);
        Assert.Equal(session.SessionId, sessionResponse.JsonBody!.RootElement.GetRequiredString("sessionId"));
        Assert.Equal(session.UserId, sessionResponse.JsonBody.RootElement.GetRequiredGuid("userId"));

        using var logoutResponse = await _fixture.ApiClient.PostJsonAsync("/api/auth/logout", new { }, session.SessionId);
        Assert.Equal(HttpStatusCode.NoContent, logoutResponse.StatusCode);

        using var reusedSessionResponse = await _fixture.ApiClient.GetAsync("/api/auth/session", session.SessionId);
        Assert.Equal(HttpStatusCode.Unauthorized, reusedSessionResponse.StatusCode);
    }

    [Fact]
    public async Task A5_MissingSessionHeader_ReturnsUnauthorized()
    {
        using var response = await _fixture.ApiClient.GetAsync("/api/auth/session");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
