namespace TodoAppAPI.WebAPI.Contracts.Requests;

public sealed class UpdateTodoRequest
{
    public string Title { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;

    public bool IsCompleted { get; init; }
}
