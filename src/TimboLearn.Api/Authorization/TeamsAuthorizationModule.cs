using Microsoft.AspNetCore.Authorization;

namespace TimboLearn.Api.Authorization;

/// <summary>
/// AUTH MODULE: Team Management Authorization
/// 
/// POLICY: CanManageTeams
/// REQUIREMENTS: User must have role claim "TeamAdmin" or "TeamManager"
/// 
/// PROTECTED ENDPOINTS:
/// - POST /api/teams - Create new team
/// - PUT /api/teams/{id} - Update team details
/// - DELETE /api/teams/{id} - Delete team
/// - POST /api/teams/{id}/members - Add user to team
/// 
/// See: CanManageTeamsHandler for implementation
/// </summary>
public class TeamsAuthorizationModule : IAuthorizationModule
{
    public void Register(AuthorizationBuilder builder)
    {
        builder.AddPolicy(Policies.CanManageTeams, policy =>
        {
            policy.Requirements.Add(new AuthorizationRequirement());
        });
    }

    public void RegisterHandlers(IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationHandler, CanManageTeamsHandler>();
    }
}
