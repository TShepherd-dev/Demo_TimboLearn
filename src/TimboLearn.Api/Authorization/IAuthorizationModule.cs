using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace TimboLearn.Api.Authorization;

public interface IAuthorizationModule
{
    void Register(AuthorizationBuilder builder);
    void RegisterHandlers(IServiceCollection services);
}
