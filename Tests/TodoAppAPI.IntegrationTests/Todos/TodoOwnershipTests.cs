using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TodoAppAPI.Application.Auth.Models;
using TodoAppAPI.Application.Common.Models;
using TodoAppAPI.Application.Todos.Models;
using TodoAppAPI.WebAPI.Contracts.Requests;
using TodoAppAPI.IntegrationTests.Infrastructure;
using Xunit;

namespace TodoAppAPI.IntegrationTests.Todos;

public class TodoOwnershipTests : IClassFixture<TodoAppApiWebApplicationFactory>
{
    private readonly TodoAppApiWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public TodoOwnershipTests(TodoAppApiWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Users_Should_Only_Access_Their_Own_Todos()
    {
        var tokenA = await RegisterAndLoginAsync($"usera-{Guid.NewGuid():N}@example.com");
        var tokenB = await RegisterAndLoginAsync($"userb-{Guid.NewGuid():N}@example.com");

        using var userAClient = CreateAuthorizedClient(tokenA);
        using var userBClient = CreateAuthorizedClient(tokenB);

        var createResponse = await userAClient.PostAsJsonAsync("/api/todos", new CreateTodoRequest
        {
            Title = "User A Todo",
            Description = "Owned by user A"
        });

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

        var createdTodo = await createResponse.Content.ReadFromJsonAsync<TodoDto>();
        Assert.NotNull(createdTodo);

        var userAListResponse = await userAClient.GetAsync("/api/todos");
        Assert.Equal(HttpStatusCode.OK, userAListResponse.StatusCode);
        var userATodos = await userAListResponse.Content.ReadFromJsonAsync<PagedResult<TodoDto>>();
        Assert.NotNull(userATodos);
        Assert.Single(userATodos!.Items);
        Assert.Equal(createdTodo!.Id, userATodos.Items.Single().Id);

        var userBListResponse = await userBClient.GetAsync("/api/todos");
        Assert.Equal(HttpStatusCode.OK, userBListResponse.StatusCode);
        var userBTodos = await userBListResponse.Content.ReadFromJsonAsync<PagedResult<TodoDto>>();
        Assert.NotNull(userBTodos);
        Assert.Empty(userBTodos!.Items);

        Assert.Equal(HttpStatusCode.OK, (await userAClient.GetAsync($"/api/todos/{createdTodo.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.Forbidden, (await userBClient.GetAsync($"/api/todos/{createdTodo.Id}")).StatusCode);

        Assert.Equal(HttpStatusCode.NoContent, (await userAClient.PutAsJsonAsync($"/api/todos/{createdTodo.Id}", new UpdateTodoRequest
        {
            Title = "Updated by owner",
            Description = "Updated",
            IsCompleted = true
        })).StatusCode);

        Assert.Equal(HttpStatusCode.Forbidden, (await userBClient.PutAsJsonAsync($"/api/todos/{createdTodo.Id}", new UpdateTodoRequest
        {
            Title = "Hacked",
            Description = "Should fail",
            IsCompleted = false
        })).StatusCode);

        Assert.Equal(HttpStatusCode.Forbidden, (await userBClient.DeleteAsync($"/api/todos/{createdTodo.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, (await userAClient.DeleteAsync($"/api/todos/{createdTodo.Id}")).StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, (await userAClient.GetAsync($"/api/todos/{createdTodo.Id}")).StatusCode);
    }

    [Fact]
    public async Task CreateTodo_With_Invalid_Input_Should_ReturnBadRequest()
    {
        var token = await RegisterAndLoginAsync($"invalid-{Guid.NewGuid():N}@example.com");
        using var client = CreateAuthorizedClient(token);

        var response = await client.PostAsJsonAsync("/api/todos", new CreateTodoRequest
        {
            Title = "   ",
            Description = "Invalid title"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private async Task<string> RegisterAndLoginAsync(string email)
    {
        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest
        {
            Email = email,
            Password = "Password123"
        });

        Assert.Equal(HttpStatusCode.Created, registerResponse.StatusCode);

        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login", new LoginRequest
        {
            Email = email,
            Password = "Password123"
        });

        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);

        var authResponse = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(authResponse);

        return authResponse!.Token;
    }

    private HttpClient CreateAuthorizedClient(string token)
    {
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}
