using Application.IntegrationTests;

namespace WebApp.IntegrationTests.Fixtures;

[CollectionDefinition(nameof(TestDatabaseCollection))]
public class TestDatabaseCollection : ICollectionFixture<TestDatabaseFixture> { }