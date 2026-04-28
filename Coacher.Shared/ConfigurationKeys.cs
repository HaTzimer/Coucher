namespace Coacher.Shared;

public static class ConfigurationKeys
{
    public const string ConnectionStringsSectionName = "ConnectionStrings";
    public const string RedisSection = "Redis";
    public const string AuthenticationSection = "Authentication";
    public const string UserDefaultsSection = "UserDefaults";
    public const string ExerciseDefaultsSection = "ExerciseDefaults";
    public const string TaskDefaultsSection = "TaskDefaults";
    public const string SessionIdHeader = "sessionIdHeader";
    public const string ItemsUserIdKey = "itemsUserIdKey";
    public const string ServicePrefix = "servicePrefix";
    public const string SessionIdPrefix = "sessionIdPrefix";
    public const string UserIdPrefix = "userIdPrefix";
    public const string SessionDurationInHours = "sessionDurationInHours";
    public const string SessionDurationExpansionInSeconds = "sessionDurationExpansionInSeconds";
    public const string EndPoints = "endPoints";
    public const string DefaultDatabase = "defaultDatabase";
    public const string Password = "password";
    public const string TimeoutInSeconds = "timeoutInSeconds";
    public const string StarterGlobalRole = "starterGlobalRole";
    public const string DefaultExerciseStatusId = "defaultExerciseStatusId";
    public const string DefaultExerciseTaskStatusId = "defaultExerciseTaskStatusId";
}
