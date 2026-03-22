using TodoAppAPI.WebAPI.Middleware;

namespace TodoAppAPI.WebAPI.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
    {
        return app.UseMiddleware<SecurityHeadersMiddleware>();
    }

    public static IApplicationBuilder UseRequestBodySizeLimit(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestBodySizeLimitMiddleware>();
    }
}
