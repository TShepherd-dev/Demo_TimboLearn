using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace TimboLearn.Api.Authorization;

public static class AuthorizationModuleRegistrar
{
    public static IServiceCollection AddAuthorizationModules(
        this IServiceCollection services,
        params IAuthorizationModule[] modules)
    {
        var authorizationBuilder = services.AddAuthorizationBuilder();

        foreach (var module in modules)
        {
            module.Register(authorizationBuilder);
            module.RegisterHandlers(services);
        }

        return services;
    }

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
            .Where(m => m != null)!
            .ToList();

        return services.AddAuthorizationModules(modules.ToArray());
    }
}
