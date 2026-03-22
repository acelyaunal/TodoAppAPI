using System.ComponentModel.DataAnnotations;

namespace TodoAppAPI.WebAPI.Configuration;

public sealed class CorsOptions
{
    public const string SectionName = "Cors";
    public const string PolicyName = "ConfiguredCors";

    [MinLength(1)]
    public string[] AllowedOrigins { get; init; } = [];

    public bool AllowCredentials { get; init; }
}
