using Microsoft.Extensions.Options;
using TodoAppAPI.WebAPI.Configuration;

namespace TodoAppAPI.WebAPI.Middleware;

public sealed class RequestBodySizeLimitMiddleware(
    RequestDelegate next,
    IOptions<SecurityOptions> securityOptions)
{
    private readonly long _maxRequestBodySizeBytes = securityOptions.Value.MaxRequestBodySizeBytes;

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.ContentLength is > 0 && context.Request.ContentLength > _maxRequestBodySizeBytes)
        {
            context.Response.StatusCode = StatusCodes.Status413PayloadTooLarge;
            await context.Response.WriteAsJsonAsync(new
            {
                type = "https://tools.ietf.org/html/rfc9110#section-15.5.14",
                title = "Request payload too large.",
                status = StatusCodes.Status413PayloadTooLarge,
                traceId = context.TraceIdentifier
            });
            return;
        }

        await next(context);
    }
}
