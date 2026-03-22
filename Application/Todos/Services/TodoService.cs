using Microsoft.Extensions.Logging;
using TodoAppAPI.Application.Common.Exceptions;
using TodoAppAPI.Application.Common.Interfaces;
using TodoAppAPI.Application.Common.Models;
using TodoAppAPI.Application.Interfaces.Repositories;
using TodoAppAPI.Application.Todos.Commands;
using TodoAppAPI.Application.Todos.Mappings;
using TodoAppAPI.Application.Todos.Models;
using TodoAppAPI.Domain.Entities;

namespace TodoAppAPI.Application.Todos.Services;

public class TodoService(
    ITodoRepository todoRepository,
    IUnitOfWork unitOfWork,
    ICurrentUserService currentUserService,
    TimeProvider timeProvider,
    ILogger<TodoService> logger) : ITodoService
{
    public async Task<PagedResult<TodoDto>> GetAllAsync(TodoListQuery query, CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.GetRequiredUserId();
        var items = await todoRepository.GetPagedAsync(userId, query.PageNumber, query.PageSize, cancellationToken);
        var totalCount = await todoRepository.CountAsync(userId, cancellationToken);

        return new PagedResult<TodoDto>
        {
            Items = items.Select(x => x.ToDto()).ToArray(),
            PageNumber = query.PageNumber,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<TodoDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.GetRequiredUserId();
        var todo = await todoRepository.GetByIdAsync(id, userId, cancellationToken);
        if (todo is null)
        {
            await EnsureTodoOwnershipOrExistenceAsync(id, cancellationToken);
        }

        return todo!.ToDto();
    }

    public async Task<TodoDto> CreateAsync(CreateTodoCommand command, CancellationToken cancellationToken = default)
    {
        var todo = TodoItem.Create(
            command.Title,
            command.Description,
            timeProvider.GetUtcNow().UtcDateTime,
            currentUserService.GetRequiredUserId());

        await todoRepository.AddAsync(todo, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Created todo {TodoId}", todo.Id);

        return todo.ToDto();
    }

    public async Task UpdateAsync(int id, UpdateTodoCommand command, CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.GetRequiredUserId();
        var todo = await todoRepository.GetTrackedByIdAsync(id, userId, cancellationToken);
        if (todo is null)
        {
            await EnsureTodoOwnershipOrExistenceAsync(id, cancellationToken);
        }

        todo!.Update(command.Title, command.Description, command.IsCompleted);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Updated todo {TodoId}", id);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var userId = currentUserService.GetRequiredUserId();
        var todo = await todoRepository.GetTrackedByIdAsync(id, userId, cancellationToken);
        if (todo is null)
        {
            await EnsureTodoOwnershipOrExistenceAsync(id, cancellationToken);
        }

        todoRepository.Remove(todo!);

        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Deleted todo {TodoId}", id);
    }

    private async Task EnsureTodoOwnershipOrExistenceAsync(int todoId, CancellationToken cancellationToken)
    {
        var exists = await todoRepository.ExistsAsync(todoId, cancellationToken);
        if (!exists)
        {
            throw new NotFoundException(nameof(TodoItem), todoId);
        }

        throw new ForbiddenException("You do not have access to this todo.");
    }
}
