namespace Coacher.ApiTests.Infrastructure;

[CollectionDefinition(Name)]
public sealed class ManualApiCollection : ICollectionFixture<ManualApiFixture>
{
    public const string Name = "manual-api";
}
