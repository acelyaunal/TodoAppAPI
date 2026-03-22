namespace TodoAppAPI.WebAPI.Contracts.Requests;

public sealed class GetTodosRequest
{
    public int PageNumber { get; init; } = 1;

    public int PageSize { get; init; } = 10;
}
