using Microsoft.AspNetCore.Authorization;

namespace TimboLearn.Api.Authorization;

public static class Policies
{
    public const string RequireAuthenticatedUser = "RequireAuthenticatedUser";
    public const string CanManageTeams = "CanManageTeams";
    public const string CanAssignContentCourse = "CanAssignContentCourse";
    public const string CanManageContentCourses = "CanManageContentCourses";
}

public class AuthorizationRequirement : IAuthorizationRequirement { }

public class RequireAuthenticatedUserHandler : AuthorizationHandler<AuthorizationRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AuthorizationRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

public class CanManageTeamsHandler : AuthorizationHandler<AuthorizationRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AuthorizationRequirement requirement)
    {
        var hasRole = context.User.HasClaim(c => 
            c.Type == "role" && (c.Value == "TeamAdmin" || c.Value == "TeamManager"));

        if (hasRole)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

public class CanAssignContentCourseHandler : AuthorizationHandler<AuthorizationRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AuthorizationRequirement requirement)
    {
        var hasPermission = context.User.HasClaim(c => 
            c.Type == "permission" && c.Value == "ContentCourse.Assign");

        var hasRole = context.User.HasClaim(c => 
            c.Type == "role" && (c.Value == "TeamAdmin" || c.Value == "TeamManager"));

        if (hasPermission || hasRole)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

public class CanManageContentCoursesHandler : AuthorizationHandler<AuthorizationRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AuthorizationRequirement requirement)
    {
        var hasPermission = context.User.HasClaim(c => 
            c.Type == "permission" && c.Value == "ContentCourse.Manage");

        var hasRole = context.User.HasClaim(c => 
            c.Type == "role" && c.Value == "Admin");

        if (hasPermission || hasRole)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}