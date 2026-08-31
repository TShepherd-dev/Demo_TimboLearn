# TimboLearn - Enterprise Learning Platform API

## Showcase Demo Project

**TimboLearn** is an enterprise-grade demonstration application built with **.NET 10** to illustrate modern C# backend architecture, Vertical Slice Architecture, high-performance data patterns, cloud-native orchestration, and AI integration.

This repository serves as a **portfolio showcase** for prospective employers and technical architects, demonstrating what a modern enterprise learning platform backend should/could look like.

It is meant to demonstrate the specific scenario "if I was asked to start a brand new API from scratch", how could I do that using common/standard/best industry practices, and my own experience of designing and working on a very very mature large-scale codebase.

AI Tools (OpenCode and Qwen3.5) have been used to build this. The project will compile, it is not designed to run. More work is required but it is a start.

---

## Architecture Vision

```
┌─────────────────────────────────────────┐       ┌─────────────────────────────────────────┐
│     EXTERNAL PROVIDER (Auth0 / Entra)   │       │         TIMBOLEARN APP CORE             │
├─────────────────────────────────────────┤       ├─────────────────────────────────────────┤
│  AUTHENTICATION (AuthN)                 │       │  AUTHORIZATION (AuthZ)                  │
│  - User identity verification           │ JWT   │  - Fine-grained domain permissions      │
│  - Multi-factor authentication (MFA)   ───────► │  - Team memberships & hierarchy         │
│  - Identity provider federation (SSO)   │ Token │  - Resource-level access policies       │
│  - Issues OAuth2 / OIDC Access Tokens   │       │  - ASP.NET Core Policy-based AuthZ      │
└─────────────────────────────────────────┘       └─────────────────────────────────────────┘
```

### Key Design Decisions

1. **Hybrid Authentication/Authorization**
   - **AuthN**: Offloaded to external OIDC providers (Auth0, Entra ID)
   - **AuthZ**: Application-level fine-grained permissions using ASP.NET Core policies

2. **Vertical Slice Architecture**
   - Features organized by business capability, not technical layer
   - Zero horizontal boilerplate services
   - Each endpoint is self-contained and testable

3. **High-Performance Data Access**
   - **EF Core 10**: Write operations, unit of work, migrations
   - **Dapper**: Read queries, projections, recursive CTEs for hierarchies

---

## Technology Stack

| Layer | Technology | Purpose |
|---|---|---|
| **Runtime** | .NET 10 | Latest performance & C# language features |
| **Orchestration** | .NET Aspire | Local dev orchestration, OpenTelemetry dashboard |
| **API Framework** | FastEndpoints | High-throughput, REPR-pattern endpoint slicing |
| **Data (ORM)** | EF Core 10 | Domain writes, unit of work, migrations |
| **Data (Queries)** | Dapper | Lightning-fast read queries and projection |
| **Database** | SQL Server / SQLite | Enterprise relational store / Local dev with SQLite |
| **Authentication** | JWT Bearer | OIDC token validation / Test tokens for local dev |
| **AI Integration** | Custom Agent Pattern | Training course generation from prompts |
| **Resilience** | Polly | Retry, circuit breaker, timeout handlers |
| **Testing** | xUnit + Testcontainers | Integration testing with real SQL containers |

---

## Solution Structure

```
TimboLearn.sln
├── src/
│   ├── TimboLearn.ServiceDefaults/       # Shared OpenTelemetry, Health Checks
│   ├── TimboLearn.Api/                   # Web API Host (FastEndpoints, Auth)
│   │   ├── Endpoints/                    # API endpoints (test token, etc.)
│   │   ├── Authorization/                # Policies, handlers, test token generator
│   │   └── Middleware/                   # User context middleware
│   ├── TimboLearn.Features/              # Business Slices (Vertical Architecture)
│   │   ├── Users/
│   │   │   ├── GetUserProfile/
│   │   │   └── UserProfileService.cs
│   │   ├── Teams/
│   │   │   ├── CreateTeam/
│   │   │   ├── AddUserToTeam/
│   │   │   └── GetTeamHierarchy/
│   │   └── ContentCourses/
│   │       ├── CreateContentCourse/
│   │       ├── AssignContentCourse/
│   │       └── GenerateContentCourseWithAI/
│   └── TimboLearn.Infrastructure/        # EF Core DbContext, Dapper, AI Agents
│       ├── Entities/                     # Domain entities
│       ├── Persistence/                  # DbContext, configurations, migrations
│       ├── Queries/                      # Dapper queries (TeamQueries)
│       ├── SeedData/                     # Development seed data
│       └── AI/                           # AI agent interfaces
└── tests/
    └── TimboLearn.IntegrationTests/      # WebApplicationFactory + Testcontainers
```

**Note:** The `TimboLearn.AppHost` project (.NET Aspire orchestrator) has been removed due to workload deprecation in .NET 10. The API runs standalone with SQLite for simplified local development.

---

## Getting Started

### Prerequisites

- .NET 10 SDK
- Visual Studio 2022 / Rider / VS Code
- (Optional) SQL Server (LocalDB or Express) - for production-like setup

### Quick Start - Local Development Mode (Recommended)

**No SQL Server required!** The project uses SQLite by default for easy local development and demos.

```bash
# 1. Restore dependencies
dotnet restore

# 2. Build solution
dotnet build

# 3. Run the API (SQLite database auto-created)
dotnet run --project src/TimboLearn.Api

# 4. Open Swagger UI
# Navigate to: http://localhost:5000/swagger
```

**Auto-Seeded Demo Data:**
- 10 Users (Alice through Jack)
- 2 Teams (Engineering Team with 5 users, Marketing Team with 8 users)
- 3 Content Courses (Cybersecurity, Communication, Project Management)
- 3 Course Assignments

### Testing the API

**Option 1: Generate Test Token (Easiest)**

1. In Swagger UI, call `POST /api/test-token` (no auth required)
2. Copy the returned JWT token
3. Click "Authorize" button
4. Paste: `Bearer <your-token-here>`
5. Now you can call all protected endpoints!

**Option 2: Use Auth0/Entra ID**

Configure `appsettings.json` with your Auth0 or Entra ID details.

### Database Setup

**SQLite (Default - for local development):**
```bash
# Database is auto-created as timbolearn.db in the Api project folder

# To reset: delete the .db file and re-run
rm src/TimboLearn.Api/timbolearn.db

# Migrations are auto-applied on startup in Development mode
```

**SQL Server (Optional - for production-like testing):**
```bash
# Update connection string in appsettings.json:
# "ConnectionStrings": { "TimboLearnDb": "Server=(localdb)\\mssqllocaldb;Database=TimboLearn;Trusted_Connection=True;" }

# Create migrations
dotnet ef migrations add InitialCreate --project src/TimboLearn.Infrastructure --startup-project src/TimboLearn.Api

# Update database
dotnet ef database update --project src/TimboLearn.Infrastructure --startup-project src/TimboLearn.Api
```

### API Documentation

Once running, access the Swagger UI at:
```
http://localhost:5000/swagger
```

---

## Feature Endpoints

### Users

| Method | Endpoint | Description | AuthZ Policy |
|---|---|---|---|
| `GET` | `/api/users/me` | Get current user profile (JIT provision) | RequireAuthenticatedUser |

### Teams

| Method | Endpoint | Description | AuthZ Policy |
|---|---|---|---|
| `POST` | `/api/teams` | Create team or sub-team | CanManageTeams |
| `POST` | `/api/teams/{id}/members` | Assign user to team | CanManageTeams |
| `GET` | `/api/teams/{id}/hierarchy` | Get nested team hierarchy | RequireAuthenticatedUser |

### ContentCourses

| Method | Endpoint | Description | AuthZ Policy |
|---|---|---|---|
| `POST` | `/api/content-courses` | Create content course | CanManageContentCourses |
| `POST` | `/api/content-courses/{id}/assign` | Assign content course to user/team | CanAssignContentCourse |
| `POST` | `/api/content-courses/ai-generate` | **AI Feature**: Auto-generate course | CanManageContentCourses |

---

## Authorization Policies

TimboLearn implements **policy-based authorization** with custom handlers:

| Policy | Description | Requirements |
|---|---|---|
| `RequireAuthenticatedUser` | Basic authenticated access | Valid JWT token |
| `CanManageTeams` | Create/manage teams | Role: `TeamAdmin` or `TeamManager` |
| `CanAssignContentCourse` | Assign content courses | Permission: `ContentCourse.Assign` OR Role: `TeamAdmin/Manager` |
| `CanManageContentCourses` | Create/manage content courses | Permission: `ContentCourse.Manage` OR Role: `Admin` |

### Claim Requirements

Tokens must include the following claims:
- `sub`: External identity ID (for JIT user provisioning)
- `email`: User email address
- `name`: User full name
- `role`: User role (optional, for authorization)
- `permission`: Specific permissions (optional)

**For Local Development:** Use `POST /api/test-token` endpoint to generate a valid test token with all required claims. The test token uses a symmetric signing key configured in `appsettings.json`.

---

## Domain Model

### Core Entities

**User**
- `Guid Id`, `string ExternalIdentityId`, `string Email`, `string FirstName`, `string LastName`
- Correlated with Auth0/Entra ID `sub` claim

**Team**
- Hierarchical structure with `ParentTeamId` self-reference
- Supports unlimited nesting via recursive CTE queries

**TeamMembership**
- Join entity with `TeamRole` (Member, TeamManager, TeamAdmin)

**ContentCourse**
- `Title`, `Description`, `EstimatedDurationMinutes`, `IsPublished`

**ContentCourseAssignment**
- Assigns content courses to individual users or entire teams
- Tracks `AssignmentStatus` (NotStarted, InProgress, Completed, Overdue)

---

## High-Performance Queries: Dapper Recursive CTE

Team hierarchy retrieval uses a recursive Common Table Expression:

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

This pattern enables efficient retrieval of deeply nested organizational structures in a single round-trip.

---

## AI Integration Demo

The `POST /api/content-courses/ai-generate` endpoint demonstrates AI-assisted content creation:

**Request:**
```json
{
  "prompt": "Cybersecurity Hygiene for Remote Workers",
  "desiredDurationMinutes": 90
}
```

**Response:**
```json
{
  "title": "AI-Generated Course: Cybersecurity Hygiene for Remote Workers",
  "description": "Comprehensive training course covering cybersecurity...",
  "modules": [
    "Introduction to Cybersecurity",
    "Fundamentals and Core Concepts",
    "Best Practices for Remote Work",
    "Advanced Techniques",
    "Hands-on Lab: Applying Security Measures",
    "Assessment: Knowledge Check"
  ]
}
```

---

## Development Features

The project includes several features to make local development and demos easier:

### 1. SQLite Database (Default)
- Zero setup required - database file auto-created on first run
- Connection string: `Data Source=timbolearn.db`
- Switch to SQL Server by updating `appsettings.json`

### 2. Auto-Seeded Demo Data
- **10 Users**: Alice Johnson through Jack Anderson
- **2 Teams**: 
  - Engineering Team (5 members)
  - Marketing Team (8 members, with some overlap)
- **3 Content Courses**: Pre-built training courses with assignments
- Seeding runs automatically in Development mode

### 3. Test Token Generator
- Endpoint: `POST /api/test-token`
- No authentication required
- Returns JWT valid for 24 hours
- Includes all necessary claims for testing all endpoints
- Configurable signing key in `appsettings.json`

### 4. Swagger UI Enabled
- Always enabled (not just in Development mode)
- Interactive API documentation
- Try endpoints directly from browser
- Built-in JWT token input

---

## Testing Strategy

Integration tests use **Testcontainers** to spin up real SQL Server instances:

```bash
dotnet test tests/TimboLearn.IntegrationTests
```

### Test Patterns

- WebApplicationFactory for in-process API hosting
- Testcontainers.MsSql for isolated database instances
- FluentAssertions for expressive test assertions

---

## Why This Architecture?

### Vertical Slice Benefits

✅ **Discoverability**: All code for a feature lives together  
✅ **Testability**: Endpoints are isolated and mockable  
✅ **Deployability**: Slices can be independently versioned  
✅ **Onboarding**: New developers find features quickly  

### Hybrid Auth Strategy

✅ **Security**: Industry-standard identity providers handle AuthN  
✅ **Flexibility**: Application controls fine-grained permissions  
✅ **Performance**: No token bloat from excessive claims  
✅ **Maintainability**: Authorization logic lives with domain code  

### EF Core + Dapper Hybrid

✅ **Productivity**: EF Core for CRUD and migrations  
✅ **Performance**: Dapper for complex reads and projections  
✅ **Best of Both**: Leverages strengths of each tool  

---

## Troubleshooting

**Database Issues:**
```bash
# Reset SQLite database
rm src/TimboLearn.Api/timbolearn.db
dotnet run --project src/TimboLearn.Api  # DB auto-recreated with seed data
```

**Migration Issues:**
```bash
# Remove last migration
dotnet ef migrations remove --project src/TimboLearn.Infrastructure

# Recreate migration
dotnet ef migrations add InitialCreate --project src/TimboLearn.Infrastructure --startup-project src/TimboLearn.Api
```

**Authentication Issues:**
- Ensure you have a valid JWT token (use `/api/test-token` endpoint)
- Token must include `email`, `name`, and `sub` claims
- For role-based auth, include `role` claim with values: `TeamAdmin`, `TeamManager`, or `Member`

---

## About This Demo

**Purpose**: This is a **showcase repository** demonstrating modern .NET enterprise architecture patterns. The code is designed to:

- Compile successfully
- Illustrate best practices
- Serve as a conversation starter in technical interviews
- Demonstrate architectural decision-making

**Note**: This is a demonstration project. For production use, additional considerations (logging, monitoring, rate limiting, etc.) would be required.

---

## Author

Built as a portfolio showcase by **TimboLearn** demonstrating:
- Vertical Slice Architecture
- .NET 10 features
- Enterprise authentication/authorization patterns
- High-performance data access (EF Core + Dapper)
- AI integration patterns
- Developer-friendly local setup (SQLite, seed data, test tokens)

---

*TimboLearn - Modern Enterprise Learning Platform Reference Architecture*

**Quick Start:** `dotnet run --project src/TimboLearn.Api` then visit http://localhost:5000/swagger
