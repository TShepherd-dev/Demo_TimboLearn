namespace TimboLearn.Features.Teams.AddUserToTeam;

public class AddUserToTeamEndpoint : Endpoint<AddUserToTeamRequest>
{
    private readonly ITeamService _teamService;

    public AddUserToTeamEndpoint(ITeamService teamService)
    {
        _teamService = teamService;
    }

    public override void Configure()
    {
        Post("/api/teams/{id}/members");
        Policies("RequireAuthenticatedUser", "CanManageTeams");
        Summary(s => {
            s.Summary = "Assigns a user to a team with a specific role";
            s.ExampleRequest = new AddUserToTeamRequest(
                Guid.NewGuid(),
                TeamRole.TeamManager
            );
        });
    }

    public override async Task HandleAsync(AddUserToTeamRequest req, CancellationToken ct)
    {
        var teamId = Route<Guid>("id");
        await _teamService.AddUserToTeamAsync(teamId, req.UserId, req.Role, ct);
        await SendNoContentAsync();
    }
}
