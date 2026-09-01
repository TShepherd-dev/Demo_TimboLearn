using TimboLearn.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace TimboLearn.Features.Users.GetUserProfile;

/// <summary>
/// ENDPOINT: GET /api/users/me
/// 
/// PURPOSE: Retrieve current authenticated user's profile with team memberships.
/// 
/// AUTHORIZATION: Requires "RequireAuthenticatedUser" policy (any valid JWT)
/// 
/// KEY FEATURES:
/// - Just-In-Time (JIT) provisioning: Creates user record on first login
/// - Claims extraction: sub, email, name from JWT token
/// - Includes team memberships with role information
/// 
/// EXAMPLE RESPONSE:
/// {
///   "id": 1,
///   "email": "alice.johnson@example.com",
///   "firstName": "Alice",
///   "lastName": "Johnson",
///   "isActive": true,
///   "teamMemberships": [
///     { "teamId": 1, "teamName": "Engineering Team", "role": "TeamAdmin" }
///   ]
/// }
/// </summary>
public class GetUserProfileEndpoint : EndpointWithoutRequest<UserProfileResponse>
{
    private readonly IUserProfileService _userService;

    public GetUserProfileEndpoint(IUserProfileService userService)
    {
        _userService = userService;
    }

    public override void Configure()
    {
        Get("/api/users/me");
        Policies("RequireAuthenticatedUser");
        Summary(s =>
        {
            s.Summary = "Retrieves current authenticated user details";
            s.Description = "Supports Just-In-Time (JIT) provisioning upon initial token validation.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        // Extract user identity from JWT claims
        var externalId = User.GetSubjectId() 
            ?? throw new BadHttpRequestException("Missing identity claim: sub or nameidentifier");

        var email = User.GetEmail() ?? string.Empty;
        var name = User.GetName() ?? email.Split('@').First();

        // Get or create user (JIT provisioning)
        var profile = await _userService.GetOrProvisionUserAsync(externalId, email, name, ct);
        await SendOkAsync(profile, ct);
    }
}
