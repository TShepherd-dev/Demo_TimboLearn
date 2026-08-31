using TimboLearn.Infrastructure.Persistence;
using TimboLearn.Infrastructure.Queries;
using Microsoft.EntityFrameworkCore;

namespace TimboLearn.Features.Teams;

public interface ITeamService
{
    Task<CreateTeamResponse> CreateTeamAsync(
        string name,
        string code,
        string? description,
        Guid? parentTeamId,
        CancellationToken cancellationToken = default);

    Task AddUserToTeamAsync(
        Guid teamId,
        Guid userId,
        TeamRole role,
        CancellationToken cancellationToken = default);

    Task<TeamHierarchyResponse?> GetHierarchyAsync(
        Guid teamId,
        CancellationToken cancellationToken = default);
}

public class TeamService : ITeamService
{
    private readonly TimboLearnDbContext _dbContext;
    private readonly TeamQueries _queries;

    public TeamService(TimboLearnDbContext dbContext, TeamQueries queries)
    {
        _dbContext = dbContext;
        _queries = queries;
    }

    public async Task<CreateTeamResponse> CreateTeamAsync(
        string name,
        string code,
        string? description,
        Guid? parentTeamId,
        CancellationToken cancellationToken = default)
    {
        var team = new Team
        {
            Id = Guid.NewGuid(),
            Name = name,
            Code = code,
            Description = description,
            ParentTeamId = parentTeamId
        };

        _dbContext.Teams.Add(team);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CreateTeamResponse(
            team.Id,
            team.Name,
            team.Code,
            team.Description,
            team.ParentTeamId
        );
    }

    public async Task AddUserToTeamAsync(
        Guid teamId,
        Guid userId,
        TeamRole role,
        CancellationToken cancellationToken = default)
    {
        var membership = new TeamMembership
        {
            UserId = userId,
            TeamId = teamId,
            Role = role,
            AssignedAtUtc = DateTime.UtcNow
        };

        _dbContext.TeamMemberships.Add(membership);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<TeamHierarchyResponse?> GetHierarchyAsync(
        Guid teamId,
        CancellationToken cancellationToken = default)
    {
        var team = await _dbContext.Teams.FindAsync(teamId);
        if (team == null) return null;

        var flatHierarchy = await _queries.GetTeamHierarchyAsync(teamId);
        var hierarchyList = flatHierarchy.ToList();

        return BuildHierarchyTree(hierarchyList, teamId);
    }

    private TeamHierarchyResponse BuildHierarchyTree(
        List<Infrastructure.Queries.TeamFlatDto> flatList,
        Guid parentId)
    {
        var parent = flatList.First(x => x.Id == parentId);
        var children = flatList
            .Where(x => x.ParentTeamId == parentId)
            .Select(x => BuildHierarchyTree(flatList, x.Id))
            .ToList();

        return new TeamHierarchyResponse(
            parent.Id,
            parent.Name,
            parent.Code,
            parent.Level,
            children
        );
    }
}
