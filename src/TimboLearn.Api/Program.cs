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

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddScoped<IUserProfileService, UserProfileService>();
builder.Services.AddScoped<ITeamService, TeamService>();

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

builder.Services.AddFastEndpoints();

// Register API Explorer for OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();

// Register authorization policies and handlers using modular pattern
builder.Services.AddAuthorizationModulesFromAssembly(typeof(AuthorizationModuleRegistrar));

var app = builder.Build();

app.UseFastEndpoints();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<TimboLearnDbContext>();
    dbContext.Database.EnsureCreated();
    await Seeder.SeedAsync(dbContext);
    
    app.UseSwaggerGen();
}

app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<UserContextMiddleware>();

app.Run();
