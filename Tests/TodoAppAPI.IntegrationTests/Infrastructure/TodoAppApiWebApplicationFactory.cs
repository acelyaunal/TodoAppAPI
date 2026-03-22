using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TodoAppAPI.IntegrationTests.Infrastructure;

public sealed class TodoAppApiWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databasePath = Path.Combine(
        Path.GetTempPath(),
        $"todoapp-int-{Guid.NewGuid():N}.db");
    private readonly Dictionary<string, string?> _previousEnvironmentValues = new();

    public TodoAppApiWebApplicationFactory()
    {
        SetEnvironmentVariable("Database__ConnectionString", $"Data Source={_databasePath}");
        SetEnvironmentVariable("Database__ApplyMigrationsOnStartup", "true");
        SetEnvironmentVariable("Jwt__Issuer", "TodoAppAPI.IntegrationTests");
        SetEnvironmentVariable("Jwt__Audience", "TodoAppAPI.IntegrationTests.Client");
        SetEnvironmentVariable("Jwt__Key", "IntegrationTestsSecretKeyMustBeAtLeast32Chars!");
        SetEnvironmentVariable("Jwt__ExpirationMinutes", "60");
        SetEnvironmentVariable("Cors__AllowedOrigins__0", "http://localhost");
        SetEnvironmentVariable("Cors__AllowCredentials", "false");
        SetEnvironmentVariable("Swagger__Enabled", "false");
        SetEnvironmentVariable("AllowedHosts", "localhost");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Development");
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing && File.Exists(_databasePath))
        {
            File.Delete(_databasePath);
        }

        foreach (var pair in _previousEnvironmentValues)
        {
            Environment.SetEnvironmentVariable(pair.Key, pair.Value);
        }
    }

    private void SetEnvironmentVariable(string key, string value)
    {
        _previousEnvironmentValues[key] = Environment.GetEnvironmentVariable(key);
        Environment.SetEnvironmentVariable(key, value);
    }
}
