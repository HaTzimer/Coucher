using System.Collections.Concurrent;

namespace Coacher.ApiTests.Infrastructure;

public sealed class ManualApiFixture : IAsyncLifetime, IDisposable
{
    private const string BootstrapQuery = """
        query BootstrapData {
          users {
            id
            identityNumber
            firstName
            lastName
          }
          userRoles {
            id
            userId
            role
          }
          units {
            id
            name
          }
          exercises {
            id
            name
            createdByUserId
            participants {
              id
              userId
              role
            }
          }
          exerciseTasks {
            id
            exerciseId
            parentId
            name
          }
          closedListItems {
            id
            key
            value
            displayOrder
          }
        }
        """;

    private readonly SemaphoreSlim _scenarioLock = new(1, 1);
    private readonly ConcurrentDictionary<Guid, Guid> _roleIdsByUserId = new();
    private SeededScenario? _scenario;

    public ManualApiFixture()
    {
        Settings = ManualApiTestSettings.LoadFromEnvironment();
        ApiClient = new CoacherApiClient(Settings);
    }

    internal ManualApiTestSettings Settings { get; }

    internal CoacherApiClient ApiClient { get; }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _scenarioLock.Dispose();
        ApiClient.Dispose();
    }

    internal async Task<SeededScenario> GetScenarioAsync()
    {
        if (_scenario is not null)
            return _scenario;

        await _scenarioLock.WaitAsync();

        try
        {
            if (_scenario is not null)
                return _scenario;

            var scenario = await BuildScenarioAsync();
            _scenario = scenario;

            return scenario;
        }
        finally
        {
            _scenarioLock.Release();
        }
    }

    internal async Task<ApiResponse> CreateExerciseAsync(
        AuthenticatedSession session,
        Guid managerUserId,
        IEnumerable<ExerciseParticipantRequest>? additionalParticipants = null
    )
    {
        var scenario = await GetScenarioAsync();

        var request = new CreateExerciseRequest
        {
            Name = CreateUniqueName("automated-exercise"),
            Description = "Created by the automated API test suite.",
            StartDate = DateOnly.FromDateTime(DateTime.UtcNow.Date),
            EndDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(14)),
            TraineeUnitId = scenario.Catalog.TraineeUnitId,
            TrainerUnitId = scenario.Catalog.TrainerUnitId,
            CompressionFactor = 1.0,
            InfluencerIds = new List<Guid> { scenario.Catalog.DefaultInfluencerId },
            SectionIds = new List<Guid> { scenario.Catalog.DefaultSectionId },
            ManagerUserId = managerUserId.ToString(),
            TraineeUnitContacts = new List<ExerciseUnitContactRequest>
            {
                new()
                {
                    FirstName = "Trainee",
                    LastName = "Contact",
                    PhoneNumber = "0500000001"
                }
            },
            TrainerUnitContacts = new List<ExerciseUnitContactRequest>
            {
                new()
                {
                    FirstName = "Trainer",
                    LastName = "Contact",
                    PhoneNumber = "0500000002"
                }
            },
            AdditionalParticipants = additionalParticipants?.ToList() ?? new List<ExerciseParticipantRequest>()
        };

        var response = await ApiClient.PostJsonAsync("/api/exercise/create/single", request, session.SessionId);

        return response;
    }

    internal async Task<ApiResponse> CreateExerciseTaskAsync(
        AuthenticatedSession session,
        Guid exerciseId,
        IEnumerable<Guid>? responsibleUserIds = null,
        IEnumerable<Guid>? dependsOnTaskIds = null
    )
    {
        var scenario = await GetScenarioAsync();

        var request = new CreateExerciseTaskRequest
        {
            ExerciseId = exerciseId,
            SeriesId = scenario.Catalog.DefaultTaskSeriesId,
            CategoryId = scenario.Catalog.DefaultTaskCategoryId,
            Name = CreateUniqueName("automated-task"),
            Description = "Created by the automated API test suite.",
            Notes = "Automated notes.",
            DueDate = DateTime.UtcNow.AddDays(5),
            ResponsibleUserIds = responsibleUserIds?.Select(item => item.ToString()).ToList(),
            DependsOnTaskIds = dependsOnTaskIds?.Select(item => item.ToString()).ToList()
        };

        var response = await ApiClient.PostJsonAsync("/api/exercise-task/create/single", request, session.SessionId);

        return response;
    }

    internal async Task<ApiResponse> CreateClosedListItemAsync(AuthenticatedSession session, string key)
    {
        var request = new CreateClosedListItemRequest
        {
            Key = key,
            Value = CreateUniqueName("automated-value"),
            Description = "Created by the automated API test suite.",
            DisplayOrder = 99
        };

        var response = await ApiClient.PostJsonAsync("/api/admin/closed-list-item/create/single", request, session.SessionId);

        return response;
    }

    internal async Task<ApiResponse> CreateTaskTemplateAsync(AuthenticatedSession session)
    {
        var scenario = await GetScenarioAsync();

        var request = new CreateTaskTemplateRequest
        {
            TemplateKey = CreateUniqueName("template-root"),
            Name = CreateUniqueName("automated-template"),
            SeriesId = scenario.Catalog.DefaultTaskSeriesId,
            CategoryId = scenario.Catalog.DefaultTaskCategoryId,
            Description = "Created by the automated API test suite.",
            Notes = "Automated notes.",
            DefaultTimeBeforeExerciseToStart = TimeSpan.FromDays(7),
            InfluencerIds = new List<Guid> { scenario.Catalog.DefaultInfluencerId },
            Children = new List<CreateTaskTemplateChildRequest>
            {
                new()
                {
                    TemplateKey = CreateUniqueName("child-a"),
                    Name = CreateUniqueName("child-a-name"),
                    SeriesId = scenario.Catalog.DefaultTaskSeriesId,
                    CategoryId = scenario.Catalog.DefaultTaskCategoryId,
                    DefaultTimeBeforeExerciseToStart = TimeSpan.FromDays(3)
                },
                new()
                {
                    TemplateKey = CreateUniqueName("child-b"),
                    Name = CreateUniqueName("child-b-name"),
                    SeriesId = scenario.Catalog.DefaultTaskSeriesId,
                    CategoryId = scenario.Catalog.DefaultTaskCategoryId,
                    DefaultTimeBeforeExerciseToStart = TimeSpan.FromDays(1),
                    DependsOnTemplateKeys = new List<string> { "child-a" }
                }
            }
        };

        request.Children[0].TemplateKey = "child-a";
        request.Children[1].TemplateKey = "child-b";

        var response = await ApiClient.PostJsonAsync("/api/admin/task-template/create/single", request, session.SessionId);

        return response;
    }

    private async Task<SeededScenario> BuildScenarioAsync()
    {
        var seedResponse = await ApiClient.PostJsonAsync("/api/mock/seed", new SeedMocksRequest
        {
            ResetExisting = true,
            UserCount = 12,
            ExerciseCount = 3,
            AdditionalParticipantsPerExercise = 4,
            TaskTemplateCount = 12,
            TasksPerExercise = 10,
            NotificationsPerUser = 2
        });

        Assert.Equal(HttpStatusCode.OK, seedResponse.StatusCode);
        Assert.NotNull(seedResponse.JsonBody);

        var adminSession = await LoginAsync("300000000", "PN-1000");
        var user1Session = await LoginAsync("300000001", "PN-1001");
        var user2Session = await LoginAsync("300000002", "PN-1002");
        var user3Session = await LoginAsync("300000003", "PN-1003");
        var unrelatedUserSession = await LoginAsync("300000004", "PN-1004");

        var bootstrapResponse = await ApiClient.GraphQlAsync(BootstrapQuery, adminSession.SessionId);
        Assert.Equal(HttpStatusCode.OK, bootstrapResponse.StatusCode);
        Assert.NotNull(bootstrapResponse.JsonBody);

        var bootstrapData = bootstrapResponse.JsonBody!.RootElement.GetRequiredProperty("data");
        var catalog = ParseCatalog(bootstrapData);

        foreach (var roleRow in bootstrapData.GetArrayItems("userRoles"))
        {
            var userId = roleRow.GetRequiredGuid("userId");
            var roleId = roleRow.GetRequiredGuid("id");

            _roleIdsByUserId[userId] = roleId;
        }

        var auditorUserId = user3Session.UserId;
        Assert.True(_roleIdsByUserId.TryGetValue(auditorUserId, out var auditorRoleId), "Expected a seeded role row for the future auditor.");

        using var promoteAuditorResponse = await ApiClient.PutJsonAsync(
            $"/api/admin/user-role/update/{auditorRoleId}",
            new UpdateUserRoleRequest
            {
                UserId = auditorUserId,
                Role = GlobalRole.Auditor
            },
            adminSession.SessionId
        );

        Assert.Equal(HttpStatusCode.OK, promoteAuditorResponse.StatusCode);

        var auditorSession = await LoginAsync("300000003", "PN-1003");

        var scenario = new SeededScenario
        {
            SeedResponse = seedResponse.JsonBody!.RootElement.Clone(),
            AdminSession = adminSession,
            User1Session = user1Session,
            User2Session = user2Session,
            AuditorSession = auditorSession,
            UnrelatedUserSession = unrelatedUserSession,
            Catalog = catalog,
            AuditorRoleId = auditorRoleId
        };

        return scenario;
    }

    private static SeedCatalog ParseCatalog(JsonElement bootstrapData)
    {
        var users = bootstrapData.GetArrayItems("users")
            .Select(item => new SeedUser
            {
                Id = item.GetRequiredGuid("id"),
                IdentityNumber = item.GetRequiredString("identityNumber"),
                FirstName = item.GetRequiredString("firstName"),
                LastName = item.GetRequiredString("lastName")
            })
            .ToList();

        var units = bootstrapData.GetArrayItems("units")
            .Select(item => new SeedUnit
            {
                Id = item.GetRequiredGuid("id"),
                Name = item.GetRequiredString("name")
            })
            .ToList();

        var closedListItems = bootstrapData.GetArrayItems("closedListItems")
            .Select(item => new SeedClosedListItem
            {
                Id = item.GetRequiredGuid("id"),
                Key = item.GetRequiredString("key"),
                Value = item.GetRequiredString("value")
            })
            .ToList();

        var closedListItemsByKey = closedListItems
            .GroupBy(item => item.Key)
            .ToDictionary(group => group.Key, group => group.ToList());

        Assert.True(units.Count >= 2, "Expected at least two seeded units.");
        Assert.True(closedListItemsByKey.ContainsKey(ConstantValues.SectionClosedListKey), "Expected seeded section closed-list values.");
        Assert.True(closedListItemsByKey.ContainsKey(ConstantValues.InfluencerClosedListKey), "Expected seeded influencer closed-list values.");
        Assert.True(closedListItemsByKey.ContainsKey(ConstantValues.TaskSeriesClosedListKey), "Expected seeded task series closed-list values.");
        Assert.True(closedListItemsByKey.ContainsKey(ConstantValues.TaskCategoryClosedListKey), "Expected seeded task category closed-list values.");

        var catalog = new SeedCatalog
        {
            Users = users,
            Units = units,
            ClosedListItems = closedListItems,
            DefaultSectionId = closedListItemsByKey[ConstantValues.SectionClosedListKey][0].Id,
            DefaultInfluencerId = closedListItemsByKey[ConstantValues.InfluencerClosedListKey][0].Id,
            DefaultTaskSeriesId = closedListItemsByKey[ConstantValues.TaskSeriesClosedListKey][0].Id,
            DefaultTaskCategoryId = closedListItemsByKey[ConstantValues.TaskCategoryClosedListKey][0].Id,
            DefaultTaskStatusId = ConstantValues.MockDefaultExerciseTaskStatusId,
            DefaultExerciseStatusId = ConstantValues.MockDefaultExerciseStatusId,
            TrainerUnitId = units[0].Id,
            TraineeUnitId = units[1].Id
        };

        return catalog;
    }

    private async Task<AuthenticatedSession> LoginAsync(string identityNumber, string passwordOrPersonalNumber)
    {
        using var response = await ApiClient.PostJsonAsync("/api/auth/login", new LoginRequest
        {
            IdentityNumber = identityNumber,
            PasswordOrPersonalNumber = passwordOrPersonalNumber
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(response.JsonBody);

        var session = JsonSerializer.Deserialize<AuthenticatedSession>(response.Body, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return session ?? throw new Xunit.Sdk.XunitException("Expected login response body to deserialize into AuthenticatedSession.");
    }

    private static string CreateUniqueName(string prefix)
    {
        var uniqueName = $"{prefix}-{Guid.NewGuid():N}"[..Math.Min(prefix.Length + 9, prefix.Length + 9)];

        return uniqueName;
    }
}

internal sealed class SeededScenario
{
    public required JsonElement SeedResponse { get; init; }

    public required AuthenticatedSession AdminSession { get; init; }

    public required AuthenticatedSession User1Session { get; init; }

    public required AuthenticatedSession User2Session { get; init; }

    public required AuthenticatedSession AuditorSession { get; init; }

    public required AuthenticatedSession UnrelatedUserSession { get; init; }

    public required SeedCatalog Catalog { get; init; }

    public required Guid AuditorRoleId { get; init; }
}

internal sealed class SeedCatalog
{
    public required List<SeedUser> Users { get; init; }

    public required List<SeedUnit> Units { get; init; }

    public required List<SeedClosedListItem> ClosedListItems { get; init; }

    public required Guid TrainerUnitId { get; init; }

    public required Guid TraineeUnitId { get; init; }

    public required Guid DefaultSectionId { get; init; }

    public required Guid DefaultInfluencerId { get; init; }

    public required Guid DefaultTaskSeriesId { get; init; }

    public required Guid DefaultTaskCategoryId { get; init; }

    public required Guid DefaultTaskStatusId { get; init; }

    public required Guid DefaultExerciseStatusId { get; init; }
}

internal sealed class SeedUser
{
    public required Guid Id { get; init; }

    public required string IdentityNumber { get; init; }

    public required string FirstName { get; init; }

    public required string LastName { get; init; }
}

internal sealed class SeedUnit
{
    public required Guid Id { get; init; }

    public required string Name { get; init; }
}

internal sealed class SeedClosedListItem
{
    public required Guid Id { get; init; }

    public required string Key { get; init; }

    public required string Value { get; init; }
}
