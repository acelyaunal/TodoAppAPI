using Microsoft.Extensions.DependencyInjection;
using TodoAppAPI.Application.Auth.Services;
using TodoAppAPI.Application.Todos.Services;

namespace TodoAppAPI.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITodoService, TodoService>();
        return services;
    }
}
