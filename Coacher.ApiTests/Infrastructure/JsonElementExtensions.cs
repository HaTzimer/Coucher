namespace Coacher.ApiTests.Infrastructure;

internal static class JsonElementExtensions
{
    public static JsonElement GetRequiredProperty(this JsonElement element, string propertyName)
    {
        if (!element.TryGetProperty(propertyName, out var property))
            throw new Xunit.Sdk.XunitException($"Expected JSON property '{propertyName}' was missing.");

        return property;
    }

    public static string GetRequiredString(this JsonElement element, string propertyName)
    {
        var property = element.GetRequiredProperty(propertyName);
        var value = property.GetString();

        return value ?? throw new Xunit.Sdk.XunitException($"Expected JSON string '{propertyName}' was null.");
    }

    public static Guid GetRequiredGuid(this JsonElement element, string propertyName)
    {
        var value = element.GetRequiredString(propertyName);
        var wasParsed = Guid.TryParse(value, out var parsedValue);

        Assert.True(wasParsed, $"Expected '{propertyName}' to be a guid but got '{value}'.");

        return parsedValue;
    }

    public static IReadOnlyList<JsonElement> GetArrayItems(this JsonElement element, string propertyName)
    {
        var property = element.GetRequiredProperty(propertyName);
        var items = property.EnumerateArray().ToArray();

        return items;
    }
}
