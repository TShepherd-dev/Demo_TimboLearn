using Microsoft.AspNetCore.Authorization;

namespace TimboLearn.Api.Authorization;

public class ContentCoursesAuthorizationModule : IAuthorizationModule
{
    public void Register(AuthorizationBuilder builder)
    {
        builder.AddPolicy(Policies.CanAssignContentCourse, policy =>
        {
            policy.Requirements.Add(new AuthorizationRequirement());
        });

        builder.AddPolicy(Policies.CanManageContentCourses, policy =>
        {
            policy.Requirements.Add(new AuthorizationRequirement());
        });
    }

    public void RegisterHandlers(IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationHandler, CanAssignContentCourseHandler>();
        services.AddSingleton<IAuthorizationHandler, CanManageContentCoursesHandler>();
    }
}
