using Dapper;
using TimboLearn.Infrastructure.Entities;

namespace TimboLearn.Infrastructure.Queries;

public record TeamFlatDto(
    int Id,
    string Name,
    string Code,
    int? ParentTeamId,
    int Level
);

public class TeamQueries
{
    private readonly IDbConnectionFactory _connectionFactory;

    public TeamQueries(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task<IEnumerable<TeamFlatDto>> GetTeamHierarchyAsync(int parentTeamId)
    {
        const string sql = """
            WITH TeamTree AS (
                SELECT Id, Name, Code, ParentTeamId, 0 AS Level
                FROM Teams
                WHERE Id = @ParentTeamId
                UNION ALL
                SELECT t.Id, t.Name, t.Code, t.ParentTeamId, tt.Level + 1
                FROM Teams t
                INNER JOIN TeamTree tt ON t.ParentTeamId = tt.Id
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
                FROM Teams
                WHERE ParentTeamId IS NULL
                UNION ALL
                SELECT t.Id, t.Name, t.Code, t.ParentTeamId, tt.Level + 1
                FROM Teams t
                INNER JOIN TeamTree tt ON t.ParentTeamId = tt.Id
            )
            SELECT * FROM TeamTree ORDER BY Level, Name;
            """;

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<TeamFlatDto>(sql);
    }

    public async Task<TeamFlatDto?> GetTeamByIdAsync(int teamId)
    {
        const string sql = """
            SELECT Id, Name, Code, ParentTeamId, 0 AS Level
            FROM Teams
            WHERE Id = @TeamId;
            """;

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryFirstOrDefaultAsync<TeamFlatDto>(sql, new { TeamId = teamId });
    }

    public async Task<IEnumerable<int>> GetTeamIdsAsync(int userId)
    {
        const string sql = """
            SELECT TeamId
            FROM TeamMemberships
            WHERE UserId = @UserId;
            """;

        using var connection = _connectionFactory.CreateConnection();
        return await connection.QueryAsync<int>(sql, new { UserId = userId });
    }

    public async Task<IEnumerable<UserWithTeamsDto>> SearchUsersAsync(
        string? searchTerm = null,
        int? teamId = null,
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
                COALESCE(GROUP_CONCAT(tm.Name), '') AS Teams
            FROM Users u
            LEFT JOIN TeamMemberships ugm ON u.Id = ugm.UserId
            LEFT JOIN Teams tm ON ugm.TeamId = tm.Id
            WHERE (@SearchTerm IS NULL OR u.FirstName LIKE @SearchPattern OR u.LastName LIKE @SearchPattern OR u.Email LIKE @SearchPattern)
              AND (@TeamId IS NULL OR tm.Id = @TeamId)
            GROUP BY u.Id, u.Email, u.FirstName, u.LastName, u.IsActive, u.CreatedAtUtc
            ORDER BY u.LastName, u.FirstName
            LIMIT @PageSize OFFSET @Skip;
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
    int Id,
    string Email,
    string FirstName,
    string LastName,
    bool IsActive,
    DateTime CreatedAtUtc,
    string? Teams
);
