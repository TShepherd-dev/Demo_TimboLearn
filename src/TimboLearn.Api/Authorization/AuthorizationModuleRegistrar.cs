using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace TimboLearn.Api.Authorization;

/// <summary>
/// EXTENSION METHODS: Modular authorization registration
/// 
/// OPTIMIZATION: Instead of manually registering each authorization policy and handler,
/// this uses reflection to auto-discover all classes implementing IAuthorizationModule.
/// 
/// WORKFLOW:
/// 1. Call services.AddAuthorizationModulesFromAssembly(typeof(AuthorizationModuleRegistrar))
/// 2. Scans the assembly for all IAuthorizationModule implementations
/// 3. Instantiates each module and calls Register() and RegisterHandlers()
/// 
/// BENEFIT: Adding new authorization is zero-config:
/// - Create a class implementing IAuthorizationModule
/// - It's automatically picked up - no Program.cs changes required
/// - Follows Open/Closed Principle (open for extension, closed for modification)
/// </summary>
public static class AuthorizationModuleRegistrar
{
    /// <summary>
    /// Register authorization modules (policies + handlers)
    /// </summary>
    public static IServiceCollection AddAuthorizationModules(
        this IServiceCollection services,
        params IAuthorizationModule[] modules)
    {
        var authorizationBuilder = services.AddAuthorizationBuilder();

        foreach (var module in modules)
        {
            // Register policy definitions (e.g., "CanManageTeams" requires role claim)
            module.Register(authorizationBuilder);
            // Register handlers that evaluate policies (e.g., CanManageTeamsHandler)
            module.RegisterHandlers(services);
        }

        return services;
    }

    /// <summary>
    /// Auto-discover and register all authorization modules in the assembly.
    /// Uses a marker type to locate the assembly containing authorization modules.
    /// </summary>
    public static IServiceCollection AddAuthorizationModulesFromAssembly(
        this IServiceCollection services,
        Type markerType)
    {
        var assembly = markerType.Assembly;
        var modules = assembly
            .GetTypes()
            .Where(t => typeof(IAuthorizationModule).IsAssignableFrom(t) && 
                       !t.IsInterface && 
                       !t.IsAbstract)
            .Select(t => Activator.CreateInstance(t) as IAuthorizationModule)
            .Where(m => m != null)
            .Cast<IAuthorizationModule>() // Explicitly cast to non-nullable
            .ToList();

        return services.AddAuthorizationModules(modules.ToArray());
    }
}
