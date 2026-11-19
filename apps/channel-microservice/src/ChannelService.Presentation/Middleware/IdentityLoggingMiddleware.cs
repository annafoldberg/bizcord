namespace ChannelService.Presentation.Middleware;

/// <summary>
/// Logs identity information forwarded by the API Gateway.
/// </summary>
public sealed class IdentityLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<IdentityLoggingMiddleware> _logger;

    public IdentityLoggingMiddleware(
        RequestDelegate next,
        ILogger<IdentityLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var userId = context.Request.Headers["X-UserId"].ToString();
        var role = context.Request.Headers["X-Role"].ToString();

        _logger.LogInformation("Identity: UserId={UserId}, Role={Role}", userId, role);

        await _next(context);
    }
}