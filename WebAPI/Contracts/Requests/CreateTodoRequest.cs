namespace TodoAppAPI.WebAPI.Contracts.Requests;

public sealed class CreateTodoRequest
{
    public string Title { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;
}
