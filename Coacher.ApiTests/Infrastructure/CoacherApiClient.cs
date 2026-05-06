namespace Coacher.ApiTests.Infrastructure;

internal sealed class CoacherApiClient : IDisposable
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly HttpClient _httpClient;

    public CoacherApiClient(ManualApiTestSettings settings)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = settings.BaseAddress,
            Timeout = settings.Timeout
        };
    }

    public async Task<ApiResponse> GetAsync(string path, string? sessionId = null, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, path);
        AddSessionHeader(request, sessionId);

        var response = await SendAsync(request, cancellationToken);

        return response;
    }

    public async Task<ApiResponse> PostJsonAsync(
        string path,
        object? body,
        string? sessionId = null,
        CancellationToken cancellationToken = default
    )
    {
        var request = new HttpRequestMessage(HttpMethod.Post, path);
        request.Content = CreateJsonContent(body);
        AddSessionHeader(request, sessionId);

        var response = await SendAsync(request, cancellationToken);

        return response;
    }

    public async Task<ApiResponse> PutJsonAsync(
        string path,
        object? body,
        string? sessionId = null,
        CancellationToken cancellationToken = default
    )
    {
        var request = new HttpRequestMessage(HttpMethod.Put, path);
        request.Content = CreateJsonContent(body);
        AddSessionHeader(request, sessionId);

        var response = await SendAsync(request, cancellationToken);

        return response;
    }

    public async Task<ApiResponse> DeleteJsonAsync(
        string path,
        object? body,
        string? sessionId = null,
        CancellationToken cancellationToken = default
    )
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, path);
        request.Content = CreateJsonContent(body);
        AddSessionHeader(request, sessionId);

        var response = await SendAsync(request, cancellationToken);

        return response;
    }

    public async Task<ApiResponse> DeleteAsync(
        string path,
        string? sessionId = null,
        CancellationToken cancellationToken = default
    )
    {
        var request = new HttpRequestMessage(HttpMethod.Delete, path);
        AddSessionHeader(request, sessionId);

        var response = await SendAsync(request, cancellationToken);

        return response;
    }

    public async Task<ApiResponse> GraphQlAsync(
        string query,
        string? sessionId = null,
        CancellationToken cancellationToken = default
    )
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/graphql");
        request.Content = CreateJsonContent(new GraphQlRequest
        {
            Query = query
        });
        AddSessionHeader(request, sessionId);

        var response = await SendAsync(request, cancellationToken);

        return response;
    }

    private static void AddSessionHeader(HttpRequestMessage request, string? sessionId)
    {
        if (!string.IsNullOrWhiteSpace(sessionId))
            request.Headers.Add("session-id", sessionId);
    }

    private static StringContent CreateJsonContent(object? body)
    {
        var payload = JsonSerializer.Serialize(body, SerializerOptions);
        var content = new StringContent(payload, Encoding.UTF8, "application/json");

        return content;
    }

    private async Task<ApiResponse> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        using var response = await _httpClient.SendAsync(request, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        JsonDocument? jsonBody = null;

        if (!string.IsNullOrWhiteSpace(body))
        {
            try
            {
                jsonBody = JsonDocument.Parse(body);
            }
            catch (JsonException)
            {
                jsonBody = null;
            }
        }

        var apiResponse = new ApiResponse
        {
            StatusCode = response.StatusCode,
            Body = body,
            RequestUri = response.RequestMessage?.RequestUri,
            JsonBody = jsonBody
        };

        return apiResponse;
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }

    private sealed class GraphQlRequest
    {
        public required string Query { get; set; }
    }
}
