using TimboLearn.Features.Teams.GetTeamHierarchy;

namespace TimboLearn.Features.Teams.CreateTeam;

public class CreateTeamEndpoint : Endpoint<CreateTeamRequest, CreateTeamResponse>
{
    private readonly ITeamService _teamService;

    public CreateTeamEndpoint(ITeamService teamService)
    {
        _teamService = teamService;
    }

    public override void Configure()
    {
        Post("/api/teams");
        Policies("RequireAuthenticatedUser", "CanManageTeams");
        Summary(s => {
            s.Summary = "Creates a new team or sub-team";
            s.ExampleRequest = new CreateTeamRequest(
                "Engineering Team",
                "ENG-TEAM",
                "All engineering staff",
                null
            );
        });
    }

    public override async Task HandleAsync(CreateTeamRequest req, CancellationToken ct)
    {
        var result = await _teamService.CreateTeamAsync(
            req.Name,
            req.Code,
            req.Description,
            req.ParentTeamId,
            ct
        );

        await SendCreatedAtAsync<GetTeamHierarchyEndpoint>(new { id = result.Id }, responseBody: result, cancellation: ct);
    }
}
