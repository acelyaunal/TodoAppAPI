using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoAppAPI.Application.Common.Models;
using TodoAppAPI.Application.Todos.Commands;
using TodoAppAPI.Application.Todos.Models;
using TodoAppAPI.Application.Todos.Services;
using TodoAppAPI.WebAPI.Contracts.Requests;

namespace TodoAppAPI.WebAPI.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class TodosController(ITodoService todoService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<TodoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PagedResult<TodoDto>>> GetAll([FromQuery] GetTodosRequest request, CancellationToken cancellationToken)
    {
        var query = new TodoListQuery(request.PageNumber, request.PageSize);
        var todos = await todoService.GetAllAsync(query, cancellationToken);
        return Ok(todos);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TodoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TodoDto>> GetById(int id, CancellationToken cancellationToken)
    {
        var todo = await todoService.GetByIdAsync(id, cancellationToken);
        return Ok(todo);
    }

    [HttpPost]
    [ProducesResponseType(typeof(TodoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TodoDto>> Create([FromBody] CreateTodoRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateTodoCommand(request.Title, request.Description);
        var createdTodo = await todoService.CreateAsync(command, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = createdTodo.Id }, createdTodo);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTodoRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateTodoCommand(request.Title, request.Description, request.IsCompleted);
        await todoService.UpdateAsync(id, command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await todoService.DeleteAsync(id, cancellationToken);
        return NoContent();
    }
}
