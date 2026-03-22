using System.ComponentModel.DataAnnotations;

namespace TodoAppAPI.WebAPI.Configuration;

public sealed class RateLimitingOptions
{
    public const string SectionName = "RateLimiting";

    [Range(1, 10_000)]
    public int PermitLimit { get; init; } = 100;

    [Range(1, 10_000)]
    public int QueueLimit { get; init; } = 0;

    [Range(1, 3600)]
    public int WindowSeconds { get; init; } = 60;
}
