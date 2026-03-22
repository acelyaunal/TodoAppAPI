namespace TodoAppAPI.Application.Todos.Commands;

public sealed record UpdateTodoCommand(string Title, string Description, bool IsCompleted);
