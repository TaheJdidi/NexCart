namespace NexCart.IntegrationTests.Infrastructure;

/// <summary>
/// Shares one API and SQL Server container across test classes, since starting a container is slow.
/// Mark a test class with <c>[Collection(ApiCollection.Name)]</c> and take a <see cref="NexCartApiFactory"/>.
/// </summary>
[CollectionDefinition(Name)]
public sealed class ApiCollection : ICollectionFixture<NexCartApiFactory>
{
    public const string Name = "API";
}
