using FastEndpoints;
using TimboLearn.Api.Authorization;

namespace TimboLearn.Api.Endpoints;

/// <summary>
/// ENDPOINT: POST /api/test-token
/// 
/// PURPOSE: Generate a test JWT token for development/testing.
/// 
/// WHY ANONYMOUS: This endpoint is called BEFORE authentication to get a token.
/// All other endpoints require authentication - this is the exception.
/// 
/// SWAGGER WORKFLOW:
/// 1. Execute this endpoint first
/// 2. Copy the returned token from response
/// 3. Click the 🔒 Authorize button (top right)
/// 4. Paste: Bearer &lt;token&gt;
/// 5. Now all protected endpoints will auto-include the token!
/// 
/// TOKEN DETAILS:
/// - Email: demo@timbolearn.local
/// - Role: TeamAdmin (has all permissions)
/// - Valid: 24 hours
/// - Permissions: ContentCourse.Assign, ContentCourse.Manage, Team.Manage
/// </summary>
public class GenerateTestTokenEndpoint : EndpointWithoutRequest<TestTokenResponse>
{
    public override void Configure()
    {
        Post("/api/test-token");
        AllowAnonymous(); // Critical: must be callable without auth!
        Summary(s =>
        {
            s.Summary = "Generate a test JWT token";
            s.Description = "Generates a valid JWT token for testing the API without Auth0.\n\n**How to use with Swagger:**\n1. Click the 🔒 Authorize button (top right)\n2. Enter: `Bearer <paste-token-here>`\n3. Click Authorize\n4. Now all protected endpoints will include the token";
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

/// <summary>
/// RESPONSE DTO: Test token response
/// </summary>
public class TestTokenResponse
{
    /// <summary>JWT token string (starts with "eyJ...")</summary>
    public string Token { get; set; } = string.Empty;
    
    /// <summary>Token validity period (always "24 hours")</summary>
    public string ExpiresIn { get; set; } = string.Empty;
}
