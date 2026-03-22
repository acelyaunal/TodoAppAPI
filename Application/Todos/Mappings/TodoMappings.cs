using TodoAppAPI.Application.Todos.Models;
using TodoAppAPI.Domain.Entities;

namespace TodoAppAPI.Application.Todos.Mappings;

public static class TodoMappings
{
    public static TodoDto ToDto(this TodoItem todo)
    {
        return new TodoDto(
            todo.Id,
            todo.Title,
            todo.Description,
            todo.IsCompleted,
            todo.CreatedAt);
    }
}
