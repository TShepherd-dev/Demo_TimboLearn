namespace TimboLearn.Api.Middleware;

/// <summary>
/// MIDDLEWARE: UserContextMiddleware
/// 
/// PURPOSE: Debug logging for authenticated requests.
/// Extracts and logs user context from JWT claims for troubleshooting.
/// 
/// POSITION IN PIPELINE: After UseAuthentication(), before endpoint execution
/// 
/// LOGGED CLAIMS:
/// - sub: User's unique identifier (from auth provider)
/// - email: User's email address
/// - name: User's display name
/// 
/// EXAMPLE LOG OUTPUT:
/// "Authenticated request - User: test-demo@timbolearn.local, Email: demo@timbolearn.local, Name: Demo User"
/// </summary>
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
        // Only log if user is authenticated (skip anonymous endpoints)
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
