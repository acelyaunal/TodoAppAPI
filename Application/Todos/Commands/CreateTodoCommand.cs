namespace TodoAppAPI.Application.Todos.Commands;

public sealed record CreateTodoCommand(string Title, string Description);
