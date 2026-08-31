namespace TimboLearn.Api.Middleware;

public class UserContextMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<UserContextMiddleware> _logger;

    public UserContextMiddleware(RequestDelegate next, ILogger<UserContextMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.User.FindFirst("sub")?.Value;
            var email = context.User.FindFirst("email")?.Value;
            var name = context.User.FindFirst("name")?.Value;

            _logger.LogDebug(
                "Authenticated request - User: {UserId}, Email: {Email}, Name: {Name}",
                userId,
                email,
                name
            );
        }

        await _next(context);
    }
}
