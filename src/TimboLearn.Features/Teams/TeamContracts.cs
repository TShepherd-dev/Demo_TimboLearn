namespace TimboLearn.Features.Teams;

public record CreateTeamRequest(
    string Name,
    string Code,
    string? Description = null,
    int? ParentTeamId = null
);

public record CreateTeamResponse(
    int Id,
    string Name,
    string Code,
    string? Description,
    int? ParentTeamId
);

public record AddUserToTeamRequest(
    int UserId,
    TeamRole Role
);

public record TeamHierarchyResponse(
    long Id,
    string Name,
    string Code,
    long Level,
    List<TeamHierarchyResponse> SubTeams
);
