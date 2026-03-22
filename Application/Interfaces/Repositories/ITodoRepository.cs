using TodoAppAPI.Domain.Entities;

namespace TodoAppAPI.Application.Interfaces.Repositories;

public interface ITodoRepository
{
    Task<IReadOnlyCollection<TodoItem>> GetPagedAsync(int userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<int> CountAsync(int userId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default);
    Task<TodoItem?> GetByIdAsync(int id, int userId, CancellationToken cancellationToken = default);
    Task<TodoItem?> GetTrackedByIdAsync(int id, int userId, CancellationToken cancellationToken = default);
    Task AddAsync(TodoItem todo, CancellationToken cancellationToken = default);
    void Remove(TodoItem todo);
}
