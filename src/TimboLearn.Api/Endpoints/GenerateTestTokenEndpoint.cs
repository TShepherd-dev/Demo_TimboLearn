using FastEndpoints;
using TimboLearn.Api.Authorization;

namespace TimboLearn.Api.Endpoints;

public class GenerateTestTokenEndpoint : EndpointWithoutRequest<TestTokenResponse>
{
    public override void Configure()
    {
        Post("/api/test-token");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "Generate a test JWT token";
            s.Description = "Generates a valid JWT token for testing the API without Auth0";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var token = TestTokenGenerator.GenerateToken(
            email: "demo@timbolearn.local",
            firstName: "Demo",
            lastName: "User",
            role: "TeamAdmin"
        );

        await SendAsync(new TestTokenResponse
        {
            Token = token,
            ExpiresIn = "24 hours"
        }, cancellation: ct);
    }
}

public class TestTokenResponse
{
    public string Token { get; set; } = string.Empty;
    public string ExpiresIn { get; set; } = string.Empty;
}
