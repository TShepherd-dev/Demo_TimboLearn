using Microsoft.AspNetCore.Authorization;

namespace TimboLearn.Api.Authorization;

/// <summary>
/// AUTH MODULE: RequireAuthenticatedUser
/// 
/// PURPOSE: Baseline policy that requires any authenticated user.
/// Used on endpoints that need authentication but no special permissions.
/// 
/// EXAMPLE USAGE:
/// - GET /api/users/me - Any authenticated user can view their own profile
/// - GET /api/teams/{id} - Any authenticated user can view team details
/// </summary>
public class RequireAuthenticatedUserModule : IAuthorizationModule
{
    public void Register(AuthorizationBuilder builder)
    {
        builder.AddPolicy(Policies.RequireAuthenticatedUser, policy =>
        {
            policy.RequireAuthenticatedUser();
        });
    }

    public void RegisterHandlers(IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationHandler, RequireAuthenticatedUserHandler>();
    }
}
