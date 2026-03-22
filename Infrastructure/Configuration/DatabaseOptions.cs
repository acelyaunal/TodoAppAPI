using System.ComponentModel.DataAnnotations;

namespace TodoAppAPI.Infrastructure.Configuration;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    [Required]
    [MinLength(1)]
    public string ConnectionString { get; init; } = string.Empty;

    public bool ApplyMigrationsOnStartup { get; init; }
}
