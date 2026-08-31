namespace TimboLearn.Features.Teams.GetTeamHierarchy;

public class GetTeamHierarchyEndpoint : EndpointWithoutRequest<TeamHierarchyResponse>
{
    private readonly ITeamService _teamService;

    public GetTeamHierarchyEndpoint(ITeamService teamService)
    {
        _teamService = teamService;
    }

    public override void Configure()
    {
        Get("/api/teams/{id}/hierarchy");
        Policies("RequireAuthenticatedUser");
        Summary(s => {
            s.Summary = "Returns nested sub-team hierarchy tree";
            s.Description = "Uses Dapper recursive CTE for high-performance hierarchical queries.";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var teamId = Route<int>("id");
        var hierarchy = await _teamService.GetHierarchyAsync(teamId, ct);

        if (hierarchy == null)
        {
            await SendNotFoundAsync();
            return;
        }

        await SendOkAsync(hierarchy, ct);
    }
}
