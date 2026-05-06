namespace Coacher.ApiTests.Infrastructure;

internal sealed class ApiResponse : IDisposable
{
    public required HttpStatusCode StatusCode { get; init; }

    public required string Body { get; init; }

    public required Uri? RequestUri { get; init; }

    public JsonDocument? JsonBody { get; init; }

    public void Dispose()
    {
        JsonBody?.Dispose();
    }
}
