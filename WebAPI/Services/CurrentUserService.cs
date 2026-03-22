using System.Security.Claims;
using TodoAppAPI.Application.Common.Exceptions;
using TodoAppAPI.Application.Common.Interfaces;

namespace TodoAppAPI.WebAPI.Services;

public sealed class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public int GetRequiredUserId()
    {
        var user = httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            throw new UnauthorizedException("Authentication is required.");
        }

        var claimValue = user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? user.FindFirstValue(ClaimTypes.Name)
            ?? user.FindFirstValue("sub");

        if (!int.TryParse(claimValue, out var userId) || userId <= 0)
        {
            throw new UnauthorizedException("Authenticated user identifier is invalid.");
        }

        return userId;
    }
}
