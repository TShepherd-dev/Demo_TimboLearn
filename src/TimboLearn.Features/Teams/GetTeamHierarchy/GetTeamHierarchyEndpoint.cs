namespace TimboLearn.Features.Teams.GetTeamHierarchy;

/// <summary>
/// ENDPOINT: GET /api/teams/{id}/hierarchy
/// 
/// PURPOSE: Retrieve complete team hierarchy (parent team + all descendant sub-teams).
/// 
/// AUTHORIZATION: Requires "RequireAuthenticatedUser" policy (any valid JWT)
/// 
/// IMPLEMENTATION: Uses Dapper with recursive CTE for high-performance traversal.
/// Single database query returns flat result set mapped to nested DTO.
/// 
/// SQL EXAMPLE:
/// WITH TeamTree AS (
///     SELECT Id, Name, Code, ParentTeamId, 0 AS Level
///     FROM Teams
///     WHERE Id = @Id
///     UNION ALL
///     SELECT t.Id, t.Name, t.Code, t.ParentTeamId, tt.Level + 1
///     FROM Teams t
///     INNER JOIN TeamTree tt ON t.ParentTeamId = tt.Id
/// )
/// SELECT * FROM TeamTree ORDER BY Level, Name;
/// 
/// RESPONSE EXAMPLE:
/// [
///   { "id": 1, "name": "Engineering", "level": 0 },
///   { "id": 11, "name": "Frontend", "level": 1 },
///   { "id": 12, "name": "Backend", "level": 1 }
/// ]
/// </summary>
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

        await SendOkAsync(hierarchy, ct);
    }
}
