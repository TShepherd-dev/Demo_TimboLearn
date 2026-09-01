using FastEndpoints;
using FastEndpoints.Swagger;
using Microsoft.EntityFrameworkCore;
using TimboLearn.Api.Authorization;
using TimboLearn.Api.Middleware;
using TimboLearn.Infrastructure;
using TimboLearn.Infrastructure.AI;
using TimboLearn.Infrastructure.SeedData;
using TimboLearn.Features.ContentCourses;
using TimboLearn.Features.Users;
using TimboLearn.Features.Teams;
using System.Text;

// ============================================================================
// TIMBOLEARN API - ENTERPRISE LEARNING PLATFORM
// ============================================================================
// Architecture: Vertical Slice Architecture with FastEndpoints
// Auth: Hybrid AuthN (external OIDC) + AuthZ (ASP.NET Core policies)
// Data: EF Core (writes) + Dapper (reads)
// ============================================================================

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------------------------------
// DATA ACCESS LAYER
// ----------------------------------------------------------------------------
// Auto-detects SQLite vs SQL Server based on connection string format:
// - SQLite: Contains "Data Source" without "Server="
// - SQL Server: Contains "Server="
// This allows zero-config switching between dev (SQLite) and production (SQL Server)
// ----------------------------------------------------------------------------

builder.Services.AddDbContext<TimboLearnDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("TimboLearnDb") 
        ?? "Data Source=timbolearn.db";
    if (connectionString.Contains("Data Source") && !connectionString.Contains("Server="))
    {
        options.UseSqlite(connectionString);
    }
    else
    {
        options.UseSqlServer(connectionString);
    }
});

// Dapper connection factory for high-performance read queries
// Used in queries like recursive CTEs for team hierarchy traversal
builder.Services.AddScoped<IDbConnectionFactory>(sp =>
{
    var connectionString = builder.Configuration.GetConnectionString("TimboLearnDb")
        ?? "Data Source=timbolearn.db";
    if (connectionString.Contains("Data Source") && !connectionString.Contains("Server="))
    {
        return new SqliteConnectionFactory(connectionString);
    }
    else
    {
        return new SqlConnectionFactory(connectionString);
    }
});

// Dapper query service for team hierarchy (uses recursive CTE)
builder.Services.AddScoped<TeamQueries>();

// ----------------------------------------------------------------------------
// FEATURE SERVICES
// ----------------------------------------------------------------------------
// Scoped services for business logic in vertical slices
// Each service is injectable into FastEndpoints endpoints
// ----------------------------------------------------------------------------

builder.Services.AddScoped<IContentCourseAiAgent, ContentCourseAiAgent>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IContentCourseService, ContentCourseService>();

// Duplicate registration fix - services already registered above
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<ITeamService, TeamService>();

// ----------------------------------------------------------------------------
// AUTHENTICATION
// ----------------------------------------------------------------------------
// JWT Bearer authentication supporting:
// - External OIDC providers (Auth0, Entra ID) via Authority/Audience config
// - Local test tokens via hardcoded signing key (Development only)
// Claims validated: sub, email, name, role, permission
// ----------------------------------------------------------------------------

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration["Auth0:Authority"] ?? "https://dev-example.auth0.com/";
        options.Audience = builder.Configuration["Auth0:Audience"] ?? "https://timbolearn-api";
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = options.Authority,
            ValidAudience = options.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Auth0:TestToken:SigningKey"] ?? "TimboLearnDemoSigningKey2026!WhichIsLongEnough")),
            ValidIssuers = new[] { options.Authority, "https://timbolearn-test" }
        };
    });

// ----------------------------------------------------------------------------
// API FRAMEWORK
// ----------------------------------------------------------------------------
// FastEndpoints: Lightweight REPR pattern on Minimal APIs
// Benefits:
// - Zero boilerplate (no controllers/attributes)
// - Automatic validation, FluentValidation integration
// - Built-in OpenAPI/Swagger support via FastEndpoints.Swagger
// - Better testability (each endpoint is a simple class)
// ----------------------------------------------------------------------------

builder.Services.AddFastEndpoints();

// Register API Explorer for OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger/OpenAPI using FastEndpoints.Swagger
builder.Services.SwaggerDocument(options =>
{
    options.DocumentSettings = s =>
    {
        s.Title = "TimboLearn API";
        s.Description = "Enterprise Learning Platform API - Showcase Demo";
        s.Version = "v1";
    };
});

// ----------------------------------------------------------------------------
// AUTHORIZATION - MODULAR PATTERN
// ----------------------------------------------------------------------------
// Scans assembly for all classes implementing IAuthorizationModule
// Each module registers:
// 1. Policy definitions (e.g., CanManageTeams, CanManageContentCourses)
// 2. Authorization handlers (e.g., CanManageTeamsHandler)
// 
// BENEFIT: To add new authorization policies, simply:
// 1. Create a new class implementing IAuthorizationModule
// 2. It's automatically discovered and registered - no Program.cs changes needed
// 
// See: Authorization/IAuthorizationModule.cs for interface definition
// ----------------------------------------------------------------------------

builder.Services.AddAuthorizationModulesFromAssembly(typeof(AuthorizationModuleRegistrar));

var app = builder.Build();

// Configure FastEndpoints middleware
// Automatically discovers and registers all classes inheriting from Endpoint<TRequest, TResponse>
app.UseFastEndpoints();

// ----------------------------------------------------------------------------
// DATABASE INITIALIZATION (Development Only)
// ----------------------------------------------------------------------------
// Auto-creates database and applies migrations on startup
// Seeds demo data: 10 users, 2 teams, 3 courses, 3 assignments
// ----------------------------------------------------------------------------

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<TimboLearnDbContext>();
    dbContext.Database.EnsureCreated();
    await Seeder.SeedAsync(dbContext);
    
    app.UseSwaggerGen();
}

// ----------------------------------------------------------------------------
// MIDDLEWARE PIPELINE
// ----------------------------------------------------------------------------
// Order matters! Middleware executes in registration order:
// 1. Authentication - validates JWT token, builds ClaimsPrincipal
// 2. Authorization - checks policies/permissions (happens in endpoints)
// 3. UserContext - logs authenticated user context for debugging
// ----------------------------------------------------------------------------

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<UserContextMiddleware>();

app.Run();
