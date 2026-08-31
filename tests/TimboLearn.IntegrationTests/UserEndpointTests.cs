namespace TimboLearn.IntegrationTests;

[Collection("TimboLearnApi")]
public class UserEndpointTests
{
    private readonly TimboLearnApiFixture _fixture;
    private readonly HttpClient _client;

    public UserEndpointTests(TimboLearnApiFixture fixture)
    {
        _fixture = fixture;
        _client = fixture.CreateFactory().CreateClient();
    }

    [Fact(Skip = "Requires authentication token setup")]
    public async Task GetUserProfile_WithValidToken_ReturnsUserProfile()
    {
        // Arrange
        // Requires JWT token setup in test harness

        // Act
        // var response = await _client.GetAsync("/api/users/me");

        // Assert
        // response.EnsureSuccessStatusCode();
        // var profile = await response.Content.ReadFromJsonAsync<UserProfileResponse>();
        // profile.Should().NotBeNull();

        await Task.CompletedTask;
    }

    [Fact]
    public void PlaceholderTest()
    {
        true.Should().BeTrue();
    }
}
