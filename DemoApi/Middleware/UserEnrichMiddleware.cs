namespace DemoApi.Middleware;

public class UserEnrichMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext context, ILogger<UserEnrichMiddleware> logger)
    {
        var user = context.User;

        // Use the HTTP context to get or set user data and add context to the error logs in services like Application Insights
        logger.LogInformation(user.Identity.IsAuthenticated ? $"User is authenticated." : "User is not authenticated.");

        await next(context);
    }
}
