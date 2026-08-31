using Microsoft.AspNetCore.Authorization;

namespace TimboLearn.Api.Authorization;

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
