namespace Coacher.ApiTests.Infrastructure;

internal sealed class ManualApiTestSettings
{
    public required Uri BaseAddress { get; init; }

    public required TimeSpan Timeout { get; init; }

    public static ManualApiTestSettings LoadFromEnvironment()
    {
        var baseUrl = Environment.GetEnvironmentVariable("COACHER_API_BASE_URL");

        if (string.IsNullOrWhiteSpace(baseUrl))
            baseUrl = "http://localhost:8080";

        var timeoutSecondsText = Environment.GetEnvironmentVariable("COACHER_API_TIMEOUT_SECONDS");
        var timeoutSeconds = 30;

        if (!string.IsNullOrWhiteSpace(timeoutSecondsText) && int.TryParse(timeoutSecondsText, out var parsedTimeoutSeconds))
            timeoutSeconds = parsedTimeoutSeconds;

        var settings = new ManualApiTestSettings
        {
            BaseAddress = new Uri(baseUrl, UriKind.Absolute),
            Timeout = TimeSpan.FromSeconds(timeoutSeconds)
        };

        return settings;
    }
}
