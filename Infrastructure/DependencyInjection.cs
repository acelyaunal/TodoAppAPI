using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TodoAppAPI.Application.Common.Interfaces;
using TodoAppAPI.Application.Interfaces.Repositories;
using TodoAppAPI.Infrastructure.Authentication;
using TodoAppAPI.Infrastructure.Configuration;
using TodoAppAPI.Infrastructure.Data;
using TodoAppAPI.Infrastructure.Repositories;

namespace TodoAppAPI.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<JwtOptions>()
            .Bind(configuration.GetSection(JwtOptions.SectionName))
            .ValidateDataAnnotations()
            .Validate(options =>
                    !string.Equals(
                        options.Key,
                        "ChangeThisDevelopmentKeyToASecure32CharMinimumSecret",
                        StringComparison.Ordinal),
                "Jwt:Key must not use the placeholder value.")
            .Validate(options => !string.IsNullOrWhiteSpace(options.Key), "Jwt:Key is required.")
            .Validate(options => options.Key.Length >= 32, "Jwt:Key must be at least 32 characters long.")
            .ValidateOnStart();

        services.AddOptions<DatabaseOptions>()
            .Bind(configuration.GetSection(DatabaseOptions.SectionName))
            .ValidateDataAnnotations()
            .Validate(options => !string.IsNullOrWhiteSpace(options.ConnectionString), "Database:ConnectionString is required.")
            .ValidateOnStart();

        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            var databaseOptions = serviceProvider.GetRequiredService<IOptions<DatabaseOptions>>().Value;
            options.UseSqlite(databaseOptions.ConnectionString);
        });

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITodoRepository, TodoRepository>();
        services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ApplicationDbContext>());
        services.AddScoped<IPasswordHasher, PasswordHasherAdapter>();
        services.AddScoped<ITokenGenerator, JwtTokenGenerator>();

        return services;
    }
}
