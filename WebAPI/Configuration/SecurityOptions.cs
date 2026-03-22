using System.ComponentModel.DataAnnotations;

namespace TodoAppAPI.WebAPI.Configuration;

public sealed class SecurityOptions
{
    public const string SectionName = "Security";

    [Range(1, 104_857_600)]
    public long MaxRequestBodySizeBytes { get; init; } = 1_048_576;
}
