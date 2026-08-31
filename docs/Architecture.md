# TimboLearn Architecture

This document provides a deep dive into the technical architecture and design decisions behind TimboLearn.

## System Overview

```
┌─────────────────────────────────────────┐       ┌─────────────────────────────────────────┐
│     EXTERNAL PROVIDER (Auth0 / Entra)   │       │         TIMBOLEARN API                  │
├─────────────────────────────────────────┤       ├─────────────────────────────────────────┤
│  AUTHENTICATION (AuthN)                 │       │  AUTHORIZATION (AuthZ)                  │
│  - User identity verification           │ JWT   │  - Policy-based permissions             │
│  - Multi-factor authentication (MFA)   ───────► │  - Team memberships & hierarchy         │
│  - Identity provider federation (SSO)   │ Token │  - Resource-level access control        │
│  - Issues OAuth2 / OIDC Access Tokens   │       │  - Custom authorization handlers        │
└─────────────────────────────────────────┘       └─────────────────────────────────────────┘
```

---

## Key Architectural Decisions

### 1. Hybrid Authentication/Authorization

**Decision:** Separate AuthN (authentication) from AuthZ (authorization)

**Why:**
- AuthN is a solved problem - use industry-standard providers (Auth0, Entra ID)
- AuthZ is domain-specific - application knows best about permissions
- Avoids token bloat from excessive claims
- Enables fine-grained, context-aware authorization

**Implementation:**
- JWT Bearer authentication validates tokens from external providers
- ASP.NET Core policy-based authorization enforces business rules
- Custom authorization handlers evaluate domain-specific requirements

### 2. Vertical Slice Architecture

**Decision:** Organize code by business feature, not technical layer

**Why:**
- **Discoverability**: All code for a feature lives together
- **Testability**: Each endpoint is isolated and independently testable
- **Deployability**: Slices can be versioned/deployed independently
- **Onboarding**: New developers find features quickly
- **No coupling**: No shared "Services" layer creating hidden dependencies

**Structure:**
```
Features/
├── Users/
│   └── GetUserProfile/
│       ├── GetUserProfileEndpoint.cs
│       ├── GetUserProfileRequest.cs
│       ├── GetUserProfileResponse.cs
│       └── UserProfileService.cs
├── Teams/
│   └── CreateTeam/
│       ├── CreateTeamEndpoint.cs
│       ├── CreateTeamRequest.cs
│       └── CreateTeamResponse.cs
└── ContentCourses/
    └── AssignContentCourse/
        ├── AssignContentCourseEndpoint.cs
        └── AssignContentCourseService.cs
```

### 3. Hybrid Data Access: EF Core + Dapper

**Decision:** Use EF Core for writes, Dapper for reads

**Why:**
- **EF Core strengths**: Change tracking, unit of work, migrations, relationships
- **Dapper strengths**: Raw performance, complex queries, recursive CTEs
- **Best of both**: Leverage each tool where it excels

**Implementation:**

```csharp
// Writes use EF Core (via DbContext)
public async Task<Team> CreateTeamAsync(Team team)
{
    _dbContext.Teams.Add(team);
    await _dbContext.SaveChangesAsync();
    return team;
}

// Reads use Dapper (via IDbConnection)
public async Task<IEnumerable<TeamDto>> GetTeamHierarchyAsync(int id)
{
    const string sql = """
        WITH TeamTree AS (
            SELECT Id, Name, Code, ParentTeamId, 0 AS Level
            FROM Teams
            WHERE Id = @Id
            UNION ALL
            SELECT t.Id, t.Name, t.Code, t.ParentTeamId, tt.Level + 1
            FROM Teams t
            INNER JOIN TeamTree tt ON t.ParentTeamId = tt.Id
        )
        SELECT * FROM TeamTree ORDER BY Level, Name;
        """;
    
    return await _connection.QueryAsync<TeamDto>(sql, new { Id = id });
}
```

---

## Technology Stack

| Layer | Technology | Purpose |
|---|---|---|
| **Runtime** | .NET 10 | Latest C# features, performance improvements |
| **API Framework** | FastEndpoints | Minimal API surface, REPR pattern, high throughput |
| **ORM (Writes)** | EF Core 10 | Change tracking, migrations, relationships |
| **Query Tool (Reads)** | Dapper | High-performance read queries, projections |
| **Database** | SQLite / SQL Server | Local dev / Production-like testing |
| **Authentication** | JWT Bearer | OAuth2 / OIDC token validation |
| **Authorization** | ASP.NET Core Policies | Fine-grained, policy-based access control |
| **Documentation** | NSwag | OpenAPI generation, Swagger UI |
| **Testing** | xUnit + Testcontainers | Integration testing with real databases |

---

## Domain Model

### Core Entities

**User**
```csharp
public class User
{
    public int Id { get; set; }
    public string ExternalIdentityId { get; set; }  // Correlates with Auth0/Entra sub claim
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    
    public ICollection<TeamMembership> TeamMemberships { get; set; }
}
```

**Team**
```csharp
public class Team
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Code { get; set; }
    public string? Description { get; set; }
    public int? ParentTeamId { get; set; }  // Self-reference for hierarchy
    
    public Team? ParentTeam { get; set; }
    public ICollection<Team> SubTeams { get; set; }
    public ICollection<TeamMembership> Memberships { get; set; }
}
```

**TeamMembership**
```csharp
public class TeamMembership
{
    public int UserId { get; set; }
    public int TeamId { get; set; }
    public TeamRole Role { get; set; }  // Member, TeamManager, TeamAdmin
    
    public User User { get; set; }
    public Team Team { get; set; }
}

public enum TeamRole
{
    Member = 0,
    TeamManager = 1,
    TeamAdmin = 2
}
```

**ContentCourse**
```csharp
public class ContentCourse
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public bool IsPublished { get; set; }
    public int EstimatedDurationMinutes { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    
    public ICollection<ContentCourseAssignment> Assignments { get; set; }
}
```

**ContentCourseAssignment**
```csharp
public class ContentCourseAssignment
{
    public int Id { get; set; }
    public int ContentCourseId { get; set; }
    public int? TargetUserId { get; set; }  // Assign to individual
    public int? TargetTeamId { get; set; }  // Assign to team
    public DateTime AssignedAtUtc { get; set; }
    public DateTime? DueDateUtc { get; set; }
    public AssignmentStatus Status { get; set; }
    
    public ContentCourse ContentCourse { get; set; }
    public User? TargetUser { get; set; }
    public Team? TargetTeam { get; set; }
}

public enum AssignmentStatus
{
    NotStarted = 0,
    InProgress = 1,
    Completed = 2,
    Overdue = 3
}
```

---

## Authorization Policies

TimboLearn implements **policy-based authorization** with custom handlers:

### Policy Definitions

```csharp
public static class Policies
{
    public const string RequireAuthenticatedUser = "RequireAuthenticatedUser";
    public const string CanManageTeams = "CanManageTeams";
    public const string CanAssignContentCourse = "CanAssignContentCourse";
    public const string CanManageContentCourses = "CanManageContentCourses";
}
```

### Policy Requirements

| Policy | Description | Requirements |
|---|---|---|
| `RequireAuthenticatedUser` | Basic authenticated access | Valid JWT with email, name, sub claims |
| `CanManageTeams` | Create/manage teams | Role: `TeamAdmin` or `TeamManager` |
| `CanAssignContentCourse` | Assign courses | Permission: `ContentCourse.Assign` OR Role: `TeamAdmin/Manager` |
| `CanManageContentCourses` | Create/manage courses | Permission: `ContentCourse.Manage` OR Role: `Admin` |

### Custom Authorization Handlers

Each policy has a corresponding handler:

```csharp
public class CanManageTeamsHandler : AuthorizationHandler<AuthorizationRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AuthorizationRequirement requirement)
    {
        var roleClaim = context.User.FindFirst("role");
        
        if (roleClaim?.Value is "TeamAdmin" or "TeamManager")
        {
            context.Succeed(requirement);
        }
        
        return Task.CompletedTask;
    }
}
```

---

## High-Performance Queries

### Recursive CTE for Team Hierarchy

```sql
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
```

**Benefits:**
- Single database round-trip
- Efficient traversal of unlimited nesting levels
- Returns flat result set easy to map to DTOs

### SQLite vs SQL Server Syntax

The codebase supports both databases:

| Feature | SQL Server | SQLite |
|---|---|---|
| String aggregation | `STRING_AGG()` | `GROUP_CONCAT()` |
| Pagination | `OFFSET ... FETCH NEXT` | `LIMIT ... OFFSET` |
| Schema prefix | `dbo.TableName` | `TableName` |
| Boolean | `bit` | `integer (0/1)` |

The DbContext automatically detects the database type and configures accordingly.

---

## AI Integration Pattern

The `GenerateContentCourseWithAI` endpoint demonstrates AI-assisted content creation:

```csharp
public interface IContentCourseAiAgent
{
    Task<GeneratedCourseContent> GenerateCourseAsync(
        string prompt,
        int desiredDurationMinutes,
        CancellationToken ct = default);
}

public class ContentCourseAiAgent : IContentCourseAiAgent
{
    public async Task<GeneratedCourseContent> GenerateCourseAsync(
        string prompt,
        int desiredDurationMinutes,
        CancellationToken ct)
    {
        // TODO: Integrate with actual AI service (Azure OpenAI, etc.)
        // For demo, returns structured mock response
        return new GeneratedCourseContent
        {
            Title = $"AI-Generated Course: {prompt}",
            Description = $"Comprehensive training course covering {prompt}...",
            Modules = new[]
            {
                "Introduction and Overview",
                "Fundamentals and Core Concepts",
                "Best Practices",
                "Advanced Techniques",
                "Hands-on Lab",
                "Assessment: Knowledge Check"
            }
        };
    }
}
```

**Production Integration:**
- Connect to Azure OpenAI Service or other LLM provider
- Implement prompt engineering for consistent output
- Add validation and human review workflow
- Cache generated content to reduce API costs

---

## Technology Choices

### Why FastEndpoints?

**FastEndpoints** is a lightweight API framework built on top of ASP.NET Core Minimal APIs that enforces the REPR (Request-Endpoint-Response) pattern.

**Why we chose it:**

1. **Vertical Slice Enforcement**: Each endpoint is a self-contained class, naturally enforcing separation of concerns
2. **Zero Boilerplate**: No controllers, no attributes cluttering your code
3. **Built-in Best Practices**: Automatic validation, FluentValidation integration, OpenAPI support
4. **Performance**: Faster than controller-based APIs due to reduced reflection and optimized pipelines
5. **Testability**: Each endpoint is a simple class that can be unit tested in isolation

**Example:**
```csharp
public class GetUserProfileEndpoint : EndpointWithoutRequest<UserProfileResponse>
{
    private readonly IUserProfileService _userService;

    public GetUserProfileEndpoint(IUserProfileService userService)
    {
        _userService = userService;
    }

    public override void Configure()
    {
        Get("/api/users/me");
        Policies("RequireAuthenticatedUser");
        Summary(s =>
        {
            s.Summary = "Get current user profile";
            s.Description = "Returns authenticated user details with JIT provisioning";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var userId = User.GetSubjectId() ?? throw new BadHttpRequestException("Missing sub claim");
        var profile = await _userService.GetProfileAsync(userId, ct);
        await SendOkAsync(profile, ct);
    }
}
```

**Alternative Considered:** Minimal APIs directly, but FastEndpoints provides better organization for larger codebases with stronger conventions.

### Why NSwag for OpenAPI/Swagger?

**NSwag** provides comprehensive OpenAPI/Swagger documentation generation with excellent .NET integration.

**Why we chose it:**

1. **FastEndpoints Integration**: Works seamlessly via `FastEndpoints.Swagger` package
2. **Rich Configuration**: Extensive customization of OpenAPI documents
3. **Client Generation**: Can generate TypeScript/C# API clients from OpenAPI spec (future use)
4. **Swagger UI**: Built-in, customizable Swagger UI middleware
5. **Mature & Stable**: Well-established library with strong community support

**Configuration:**
```csharp
// In Program.cs
builder.Services.SwaggerDocument(options =>
{
    options.DocumentSettings = s =>
    {
        s.Title = "TimboLearn API";
        s.Description = "Enterprise Learning Platform API";
        s.Version = "v1";
    };
});

app.UseSwaggerGen(); // Enables Swagger UI at /swagger
```

**Alternative Considered:** Swashbuckle (Microsoft's default), but NSwag has better FastEndpoints integration and more advanced features for client generation.

### Why SQLite for Development?

**SQLite** is the default database for local development and demos.

**Why we chose it:**

1. **Zero Setup**: No SQL Server installation required
2. **Portable**: Single file database, easy to share/reset
3. **Fast**: In-memory operations, instant startup
4. **Entity Framework Compatible**: Full EF Core support
5. **Demo-Friendly**: Delete the `.db` file to reset everything

**Production Path:** Simply change the connection string to SQL Server/Azure SQL in `appsettings.json` - no code changes required!

---

## Testing Strategy

### Integration Tests with Testcontainers

```csharp
public class TeamEndpointTests : IClassFixture<IntegrationTestFactory>
{
    private readonly HttpClient _client;
    
    [Fact]
    public async Task CreateTeam_ReturnsCreatedStatus()
    {
        // Arrange
        var request = new CreateTeamRequest
        {
            Name = "Test Team",
            Code = "TEST"
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/teams", request);
        
        // Assert
        response.Should().HaveStatusCode(HttpStatusCode.Created);
    }
}
```

**Testcontainers Benefits:**
- Real SQL Server instances in Docker containers
- Isolated, parallelizable tests
- No manual database setup required
- Production-like testing environment

---

## Next Steps

- **[Getting Started](GettingStarted.md)** - Set up and run the API
- **[Testing Guide](Testing.md)** - Test endpoints with Swagger UI
- **[Troubleshooting](Troubleshooting.md)** - Common issues and solutions
