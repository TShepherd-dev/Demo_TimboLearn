using Dapper;
using TimboLearn.Infrastructure.Entities;

namespace TimboLearn.Infrastructure.Queries;

public record TeamFlatDto(
    Guid Id,
    string Name,
    string Code,
    Guid? ParentTeamId,
    int Level
);

public class TeamQueries
{
    private readonly IDbConnectionFactory _connectionFactory;

    public TeamQueries(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<TeamFlatDto>> GetTeamHierarchyAsync(Guid parentTeamId)
    {
        const string sql = """
            WITH TeamTree AS (
                SELECT Id, Name, Code, ParentTeamId, 0 AS Level
                FROM dbo.Teams
                WHERE Id = @ParentTeamId
                UNION ALL
                SELECT g.Id, g.Name, g.Code, g.ParentTeamId, gt.Level + 1
                FROM dbo.Teams g
                INNER JOIN TeamTree gt ON g.ParentTeamId = gt.Id
            )
            SELECT * FROM TeamTree ORDER BY Level, Name;
            """;

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<TeamFlatDto>(sql, new { ParentTeamId = parentTeamId });
    }

    public async Task<IEnumerable<TeamFlatDto>> GetAllTeamsHierarchyAsync()
    {
        const string sql = """
            WITH TeamTree AS (
                SELECT Id, Name, Code, ParentTeamId, 0 AS Level
                FROM dbo.Teams
                WHERE ParentTeamId IS NULL
                UNION ALL
                SELECT g.Id, g.Name, g.Code, g.ParentTeamId, gt.Level + 1
                FROM dbo.Teams g
                INNER JOIN TeamTree gt ON g.ParentTeamId = gt.Id
            )
            SELECT * FROM TeamTree ORDER BY Level, Name;
            """;

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<TeamFlatDto>(sql);
    }

    public async Task<TeamFlatDto?> GetTeamByIdAsync(Guid teamId)
    {
        const string sql = """
            SELECT Id, Name, Code, ParentTeamId, 0 AS Level
            FROM dbo.Teams
            WHERE Id = @TeamId;
            """;

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<TeamFlatDto>(sql, new { TeamId = teamId });
    }

    public async Task<IEnumerable<Guid>> GetTeamIdsAsync(Guid userId)
    {
        const string sql = """
            SELECT TeamId
            FROM dbo.TeamMemberships
            WHERE UserId = @UserId;
            """;

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<Guid>(sql, new { UserId = userId });
    }

    public async Task<IEnumerable<UserWithTeamsDto>> SearchUsersAsync(
        string? searchTerm = null,
        Guid? teamId = null,
        int page = 1,
        int pageSize = 50)
    {
        var searchPattern = searchTerm != null ? $"%{searchTerm}%" : null;
        var skip = (page - 1) * pageSize;

        var sql = """
            SELECT 
                u.Id,
                u.Email,
                u.FirstName,
                u.LastName,
                u.IsActive,
                u.CreatedAtUtc,
                STRING_AGG(g.Name, ', ') AS Teams
            FROM dbo.Users u
            LEFT JOIN dbo.TeamMemberships ugm ON u.Id = ugm.UserId
            LEFT JOIN dbo.Teams g ON ugm.TeamId = g.Id
            WHERE (@SearchTerm IS NULL OR u.FirstName LIKE @SearchPattern OR u.LastName LIKE @SearchPattern OR u.Email LIKE @SearchPattern)
              AND (@TeamId IS NULL OR g.Id = @TeamId)
            GROUP BY u.Id, u.Email, u.FirstName, u.LastName, u.IsActive, u.CreatedAtUtc
            ORDER BY u.LastName, u.FirstName
            OFFSET @Skip ROWS FETCH NEXT @PageSize ROWS ONLY;
            """;

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<UserWithTeamsDto>(sql, new { 
            SearchTerm = searchTerm, 
            SearchPattern = searchPattern, 
            TeamId = teamId, 
            Skip = skip, 
            PageSize = pageSize 
        });
    }
}

public record UserWithTeamsDto(
    Guid Id,
    string Email,
    string FirstName,
    string LastName,
    bool IsActive,
    DateTime CreatedAtUtc,
    string? Teams
);
