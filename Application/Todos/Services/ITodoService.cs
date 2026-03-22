using TodoAppAPI.Application.Common.Models;
using TodoAppAPI.Application.Todos.Commands;
using TodoAppAPI.Application.Todos.Models;

namespace TodoAppAPI.Application.Todos.Services;

public interface ITodoService
{
    Task<PagedResult<TodoDto>> GetAllAsync(TodoListQuery query, CancellationToken cancellationToken = default);
    Task<TodoDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<TodoDto> CreateAsync(CreateTodoCommand command, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, UpdateTodoCommand command, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}
