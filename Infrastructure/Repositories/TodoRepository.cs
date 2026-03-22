using Microsoft.EntityFrameworkCore;
using TodoAppAPI.Application.Interfaces.Repositories;
using TodoAppAPI.Domain.Entities;
using TodoAppAPI.Infrastructure.Data;

namespace TodoAppAPI.Infrastructure.Repositories;

public class TodoRepository(ApplicationDbContext dbContext) : ITodoRepository
{
    public async Task<IReadOnlyCollection<TodoItem>> GetPagedAsync(int userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        return await dbContext.Todos
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)
            .ThenByDescending(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return dbContext.Todos.CountAsync(cancellationToken);
    }

    public Task<int> CountAsync(int userId, CancellationToken cancellationToken = default)
    {
        return dbContext.Todos.CountAsync(x => x.UserId == userId, cancellationToken);
    }

    public Task<bool> ExistsAsync(int id, CancellationToken cancellationToken = default)
    {
        return dbContext.Todos.AnyAsync(x => x.Id == id, cancellationToken);
    }

    public Task<TodoItem?> GetByIdAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        return dbContext.Todos
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);
    }

    public Task<TodoItem?> GetTrackedByIdAsync(int id, int userId, CancellationToken cancellationToken = default)
    {
        return dbContext.Todos.FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId, cancellationToken);
    }

    public async Task AddAsync(TodoItem todo, CancellationToken cancellationToken = default)
    {
        await dbContext.Todos.AddAsync(todo, cancellationToken);
    }

    public void Remove(TodoItem todo)
    {
        dbContext.Todos.Remove(todo);
    }
}
