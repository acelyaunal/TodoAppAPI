using TodoAppAPI.Domain.Common.Exceptions;

namespace TodoAppAPI.Domain.Entities;

public class TodoItem
{
    private TodoItem()
    {
    }

    private TodoItem(string title, string description, DateTime createdAtUtc, int userId)
    {
        Title = NormalizeTitle(title);
        Description = NormalizeDescription(description);
        CreatedAt = EnsureUtc(createdAtUtc);
        UserId = EnsureUserId(userId);
    }

    public int Id { get; private set; }

    public string Title { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public bool IsCompleted { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public int UserId { get; private set; }

    public static TodoItem Create(string title, string description, DateTime createdAtUtc, int userId)
    {
        return new TodoItem(title, description, createdAtUtc, userId);
    }

    public void Update(string title, string description, bool isCompleted)
    {
        Title = NormalizeTitle(title);
        Description = NormalizeDescription(description);
        IsCompleted = isCompleted;
    }

    private static string NormalizeTitle(string title)
    {
        var normalizedTitle = title?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(normalizedTitle))
        {
            throw new DomainValidationException("Title is required.");
        }

        if (normalizedTitle.Length > 200)
        {
            throw new DomainValidationException("Title cannot exceed 200 characters.");
        }

        return normalizedTitle;
    }

    private static string NormalizeDescription(string? description)
    {
        var normalizedDescription = description?.Trim() ?? string.Empty;
        if (normalizedDescription.Length > 1000)
        {
            throw new DomainValidationException("Description cannot exceed 1000 characters.");
        }

        return normalizedDescription;
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

    private static int EnsureUserId(int userId)
    {
        if (userId <= 0)
        {
            throw new DomainValidationException("UserId must be a positive value.");
        }

        return userId;
    }
}
