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
        Summary(s => {
            s.Summary = "Retrieves current authenticated user details";
            s.Description = "Supports Just-In-Time (JIT) provisioning upon initial token validation.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var externalId = User.FindFirst("sub")?.Value 
            ?? throw new BadHttpRequestException("Missing identity claim: sub");

        var email = User.FindFirst("email")?.Value ?? string.Empty;
        var name = User.FindFirst("name")?.Value ?? email.Split('@').First();

        var profile = await _userService.GetOrProvisionUserAsync(externalId, email, name, ct);
        await SendOkAsync(profile, ct);
    }
}
