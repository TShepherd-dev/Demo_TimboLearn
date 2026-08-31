using Microsoft.AspNetCore.Authorization;

namespace TimboLearn.Api.Authorization;

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
