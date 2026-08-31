namespace TimboLearn.Features.Teams;

public record CreateTeamRequest(
    string Name,
    string Code,
    string? Description = null,
    Guid? ParentTeamId = null
);

public record CreateTeamResponse(
    Guid Id,
    string Name,
    string Code,
    string? Description,
    Guid? ParentTeamId
);

public record AddUserToTeamRequest(
    Guid UserId,
    TeamRole Role
);

public record TeamHierarchyResponse(
    Guid Id,
    string Name,
    string Code,
    int Level,
    List<TeamHierarchyResponse> SubTeams
);
