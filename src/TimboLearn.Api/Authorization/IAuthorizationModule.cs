using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace TimboLearn.Api.Authorization;

/// <summary>
/// CONTRACT: Authorization policy module for modular authorization configuration.
/// 
/// IMPLEMENTATION GUIDE:
/// To add new authorization policies, create a new class implementing this interface:
/// 
/// 1. Define policy names as constants (for Intelli discoverability)
/// 2. In Register(): Add policies with requirements
/// 3. In RegisterHandlers(): Register authorization handlers
/// 
/// EXAMPLE:
/// public class MyFeatureAuthModule : IAuthorizationModule
/// {
///     public const string CanDoSomething = "CanDoSomething";
///     
///     public void Register(AuthorizationBuilder builder)
///     {
///         builder.AddPolicy(CanDoSomething, policy =>
///         {
///             policy.Requirements.Add(new AuthorizationRequirement());
///         });
///     }
///     
///     public void RegisterHandlers(IServiceCollection services)
///     {
///         services.AddSingleton&lt;IAuthorizationHandler, CanDoSomethingHandler&gt;();
///     }
/// }
/// 
/// The module is automatically discovered by AddAuthorizationModulesFromAssembly()
/// in Program.cs - no manual registration needed!
/// </summary>
public interface IAuthorizationModule
{
    /// <summary>
    /// Register authorization policies (e.g., CanManageTeams, CanManageContentCourses)
    /// </summary>
    void Register(AuthorizationBuilder builder);
    
    /// <summary>
    /// Register authorization handlers that evaluate policy requirements
    /// </summary>
    void RegisterHandlers(IServiceCollection services);
}
