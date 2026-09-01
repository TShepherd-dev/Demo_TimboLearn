using Microsoft.AspNetCore.Authorization;

namespace TimboLearn.Api.Authorization;

/// <summary>
/// AUTHORIZATION POLICY CONSTANTS
/// 
/// These policy names are used in endpoint configuration:
/// Example: Policies(CanManageTeams) in endpoint Configure() method
/// 
/// Policy evaluation flow:
/// 1. User presents JWT token with claims
/// 2. Policy handler checks if claims satisfy requirements
/// 3. If successful, endpoint executes; otherwise returns 403 Forbidden
/// </summary>
public static class Policies
{
    /// <summary>Requires any authenticated user (baseline auth)</summary>
    public const string RequireAuthenticatedUser = "RequireAuthenticatedUser";
    
    /// <summary>Requires TeamAdmin or TeamManager role</summary>
    public const string CanManageTeams = "CanManageTeams";
    
    /// <summary>Requires ContentCourse.Assign permission OR TeamAdmin/TeamManager role</summary>
    public const string CanAssignContentCourse = "CanAssignContentCourse";
    
    /// <summary>Requires ContentCourse.Manage permission OR Admin role</summary>
    public const string CanManageContentCourses = "CanManageContentCourses";
}

/// <summary>
/// BASE REQUIREMENT: Marker interface for custom authorization requirements.
/// Handlers check claims (role, permission) to determine if requirement is satisfied.
/// </summary>
public class AuthorizationRequirement : IAuthorizationRequirement { }

/// <summary>
/// HANDLER: RequireAuthenticatedUser
/// EVALUATION: Succeeds if user has any valid authentication
/// </summary>
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

/// <summary>
/// HANDLER: CanManageTeams
/// EVALUATION: Succeeds if user has role claim "TeamAdmin" or "TeamManager"
/// 
/// CLAIM CHECK: Looks for claim with Type="role" and Value in ["TeamAdmin", "TeamManager"]
/// </summary>
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

/// <summary>
/// HANDLER: CanAssignContentCourse
/// EVALUATION: Succeeds if user has:
/// - permission claim "ContentCourse.Assign" OR
/// - role claim "TeamAdmin" or "TeamManager"
/// 
/// This allows fine-grained permission-based access OR broad role-based access
/// </summary>
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

/// <summary>
/// HANDLER: CanManageContentCourses
/// EVALUATION: Succeeds if user has:
/// - permission claim "ContentCourse.Manage" OR
/// - role claim "Admin"
/// 
/// Note: "Admin" role is broader than "TeamAdmin" - full system access
/// </summary>
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