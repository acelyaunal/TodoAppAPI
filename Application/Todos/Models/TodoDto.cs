namespace TodoAppAPI.Application.Todos.Models;

public sealed record TodoDto(
    int Id,
    string Title,
    string Description,
    bool IsCompleted,
    DateTime CreatedAt);
