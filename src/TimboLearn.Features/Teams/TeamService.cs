using TimboLearn.Infrastructure.Persistence;
using TimboLearn.Infrastructure.Queries;
using Microsoft.EntityFrameworkCore;
using TimboLearn.Infrastructure;

namespace TimboLearn.Features.Teams;

public interface ITeamService
{
    Task<CreateTeamResponse> CreateTeamAsync(
        string name,
        string code,
        string? description,
        int? parentTeamId,
        CancellationToken cancellationToken = default);

    Task AddUserToTeamAsync(
        int teamId,
        int userId,
        TeamRole role,
        CancellationToken cancellationToken = default);

    Task<TeamHierarchyResponse> GetHierarchyAsync(
        int teamId,
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
        int? parentTeamId,
        CancellationToken cancellationToken = default)
    {
        // Validate parent team exists if provided
        if (parentTeamId.HasValue)
        {
            var parentTeam = await _dbContext.Teams.FindAsync([parentTeamId.Value], cancellationToken);
            if (parentTeam == null)
            {
                throw new NotFoundException("ParentTeam", $"Parent team with ID {parentTeamId.Value} was not found");
            }
        }

        // Check for duplicate code
        var existingTeam = await _dbContext.Teams
            .FirstOrDefaultAsync(t => t.Code == code, cancellationToken);
        
        if (existingTeam != null)
        {
            throw new ConflictException("Team", $"A team with code '{code}' already exists");
        }

        var team = new Team
        {
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
        int teamId,
        int userId,
        TeamRole role,
        CancellationToken cancellationToken = default)
    {
        // Validate team exists
        var team = await _dbContext.Teams.FindAsync([teamId], cancellationToken);
        if (team == null)
        {
            throw new NotFoundException("Team", $"Team with ID {teamId} was not found");
        }

        // Validate user exists
        var user = await _dbContext.Users.FindAsync([userId], cancellationToken);
        if (user == null)
        {
            throw new NotFoundException("User", $"User with ID {userId} was not found");
        }

        // Check for existing membership
        var existingMembership = await _dbContext.TeamMemberships
            .FirstOrDefaultAsync(m => m.UserId == userId && m.TeamId == teamId, cancellationToken);
        
        if (existingMembership != null)
        {
            throw new ConflictException("TeamMembership", $"User {userId} is already a member of team {teamId}");
        }

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

    public async Task<TeamHierarchyResponse> GetHierarchyAsync(
        int teamId,
        CancellationToken cancellationToken = default)
    {
        var team = await _dbContext.Teams.FindAsync([teamId], cancellationToken);
        if (team == null)
        {
            throw new NotFoundException("Team", $"Team with ID {teamId} was not found");
        }

        var flatHierarchy = await _queries.GetTeamHierarchyAsync(teamId);
        var hierarchyList = flatHierarchy.ToList();

        return BuildHierarchyTree(hierarchyList, teamId);
    }

    private TeamHierarchyResponse BuildHierarchyTree(
        List<Infrastructure.Queries.TeamFlatDto> flatList,
        long parentId)
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
