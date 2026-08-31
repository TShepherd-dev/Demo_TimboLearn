using TimboLearn.Infrastructure;
using Microsoft.AspNetCore.Http;

namespace TimboLearn.Features.Users.GetUserProfile;

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
        var externalId = User.GetSubjectId() 
            ?? throw new BadHttpRequestException("Missing identity claim: sub or nameidentifier");

        var email = User.GetEmail() ?? string.Empty;
        var name = User.GetName() ?? email.Split('@').First();

        var profile = await _userService.GetOrProvisionUserAsync(externalId, email, name, ct);
        await SendOkAsync(profile, ct);
    }
}
