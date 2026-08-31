namespace TimboLearn.IntegrationTests;

public class TimboLearnApiFixture : IAsyncLifetime
{
    private readonly MsSqlContainer _sqlContainer = new MsSqlBuilder().Build();
    public WebApplicationFactory<Program> CreateFactory() => new();

    public async Task InitializeAsync()
    {
        await _sqlContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _sqlContainer.StopAsync();
    }

    public string GetConnectionString() => _sqlContainer.GetConnectionString();
}

[CollectionDefinition("TimboLearnApi")]
public class TimboLearnApiCollection : ICollectionFixture<TimboLearnApiFixture>
{
}
