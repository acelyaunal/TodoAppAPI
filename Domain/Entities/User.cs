using System.Text.RegularExpressions;
using TodoAppAPI.Domain.Common.Exceptions;

namespace TodoAppAPI.Domain.Entities;

public class User
{
    private static readonly Regex EmailRegex =
        new(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private User()
    {
    }

    private User(string email, string passwordHash, DateTime createdAtUtc)
    {
        Email = NormalizeEmail(email);
        PasswordHash = EnsurePasswordHash(passwordHash);
        CreatedAt = EnsureUtc(createdAtUtc);
    }

    public int Id { get; private set; }

    public string Email { get; private set; } = string.Empty;

    public string PasswordHash { get; private set; } = string.Empty;

    public DateTime CreatedAt { get; private set; }

    public static User Create(string email, string passwordHash, DateTime createdAtUtc)
    {
        return new User(email, passwordHash, createdAtUtc);
    }

    public static string NormalizeEmail(string email)
    {
        var normalizedEmail = email?.Trim().ToLowerInvariant() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(normalizedEmail))
        {
            throw new DomainValidationException("Email is required.");
        }

        if (normalizedEmail.Length > 320)
        {
            throw new DomainValidationException("Email cannot exceed 320 characters.");
        }

        if (!EmailRegex.IsMatch(normalizedEmail))
        {
            throw new DomainValidationException("Email format is invalid.");
        }

        return normalizedEmail;
    }

    private static string EnsurePasswordHash(string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(passwordHash))
        {
            throw new DomainValidationException("Password hash is required.");
        }

        return passwordHash;
    }

    private static DateTime EnsureUtc(DateTime value)
    {
        if (value == default)
        {
            throw new DomainValidationException("CreatedAt must be provided.");
        }

        return value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
        };
    }
}
