using Microsoft.AspNetCore.Authorization;

namespace TimboLearn.Api.Authorization;

/// <summary>
/// AUTH MODULE: Content Course Authorization
/// 
/// POLICIES:
/// 1. CanAssignContentCourse - Assign courses to users/teams
///    Requirements: permission "ContentCourse.Assign" OR role "TeamAdmin"/"TeamManager"
/// 
/// 2. CanManageContentCourses - Create/edit/delete courses
///    Requirements: permission "ContentCourse.Manage" OR role "Admin"
/// 
/// PROTECTED ENDPOINTS:
/// - POST /api/content-courses - Create course (requires CanManageContentCourses)
/// - POST /api/content-courses/assign - Assign course (requires CanAssignContentCourse)
/// - POST /api/content-courses/ai-generate - AI-generate course (requires CanManageContentCourses)
/// 
/// See: CanAssignContentCourseHandler, CanManageContentCoursesHandler for implementations
/// </summary>
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
