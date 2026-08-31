using FastEndpoints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TimboLearn.Api.Authorization;
using TimboLearn.Api.Middleware;
using TimboLearn.Infrastructure;
using TimboLearn.Infrastructure.Entities;
using TimboLearn.Infrastructure.Persistence;
using TimboLearn.Infrastructure.Queries;
using TimboLearn.Infrastructure.SeedData;
using TimboLearn.Features.ContentCourses;
using TimboLearn.Features.Users;
using TimboLearn.Features.Teams;
using NSwag;
using NSwag.Generation.AspNetCore;
using Microsoft.Data.Sqlite;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<TimboLearnDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("TimboLearnDb") 
        ?? "Data Source=timbolearn.db";
    if (connectionString.Contains("Data Source"))
    {
        options.UseSqlite(connectionString);
    }
    else
    {
        options.UseSqlServer(connectionString);
    }
});

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

builder.Services.AddScoped<TeamQueries>();

builder.Services.AddScoped<IContentCourseAiAgent, ContentCourseAiAgent>();
builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IContentCourseService, ContentCourseService>();

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

builder.Services.AddAuthorizationBuilder()
    .AddPolicy(Policies.RequireAuthenticatedUser, policy =>
    {
        policy.RequireAuthenticatedUser();
    })
    .AddPolicy(Policies.CanManageTeams, policy =>
    {
        policy.Requirements.Add(new AuthorizationRequirement());
    })
    .AddPolicy(Policies.CanAssignContentCourse, policy =>
    {
        policy.Requirements.Add(new AuthorizationRequirement());
    })
    .AddPolicy(Policies.CanManageContentCourses, policy =>
    {
        policy.Requirements.Add(new AuthorizationRequirement());
    });

builder.Services.AddSingleton<IAuthorizationHandler, RequireAuthenticatedUserHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, CanManageTeamsHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, CanAssignContentCourseHandler>();
builder.Services.AddSingleton<IAuthorizationHandler, CanManageContentCoursesHandler>();

builder.Services.AddFastEndpoints();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApiDocument(options =>
{
    options.Title = "TimboLearn API";
    options.Description = "Enterprise Learning Platform API - Showcase Demo";
    options.Version = "v1";
});

var app = builder.Build();

app.UseDefaultExceptionHandler();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<TimboLearnDbContext>();
    await Seeder.SeedAsync(dbContext);
}

app.UseOpenApi();
app.UseSwaggerUi();

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<UserContextMiddleware>();

app.UseFastEndpoints();

app.Run();