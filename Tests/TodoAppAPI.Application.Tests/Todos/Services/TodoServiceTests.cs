using Microsoft.Extensions.Logging;
using Moq;
using TodoAppAPI.Application.Common.Exceptions;
using TodoAppAPI.Application.Common.Interfaces;
using TodoAppAPI.Application.Interfaces.Repositories;
using TodoAppAPI.Application.Todos.Commands;
using TodoAppAPI.Application.Todos.Services;
using TodoAppAPI.Domain.Entities;
using Xunit;

namespace TodoAppAPI.Application.Tests.Todos.Services;

public class TodoServiceTests
{
    [Fact]
    public async Task CreateAsync_Should_AddTodo_SaveChanges_AndReturnMappedDto()
    {
        var fixedUtcNow = new DateTimeOffset(2026, 3, 22, 12, 0, 0, TimeSpan.Zero);
        var timeProvider = new FixedTimeProvider(fixedUtcNow);
        var repositoryMock = new Mock<ITodoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var currentUserServiceMock = new Mock<ICurrentUserService>();
        var loggerMock = new Mock<ILogger<TodoService>>();
        TodoItem? addedTodo = null;

        currentUserServiceMock
            .Setup(x => x.GetRequiredUserId())
            .Returns(42);

        repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<TodoItem>(), It.IsAny<CancellationToken>()))
            .Callback<TodoItem, CancellationToken>((todo, _) => addedTodo = todo)
            .Returns(Task.CompletedTask);

        unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var service = new TodoService(
            repositoryMock.Object,
            unitOfWorkMock.Object,
            currentUserServiceMock.Object,
            timeProvider,
            loggerMock.Object);

        var result = await service.CreateAsync(new CreateTodoCommand("  Buy milk  ", "  From store  "));

        Assert.NotNull(addedTodo);
        Assert.Equal("Buy milk", addedTodo!.Title);
        Assert.Equal("From store", addedTodo.Description);
        Assert.Equal(fixedUtcNow.UtcDateTime, addedTodo.CreatedAt);
        Assert.Equal(42, addedTodo.UserId);

        Assert.Equal("Buy milk", result.Title);
        Assert.Equal("From store", result.Description);
        Assert.False(result.IsCompleted);
        Assert.Equal(fixedUtcNow.UtcDateTime, result.CreatedAt);

        repositoryMock.Verify(x => x.AddAsync(It.IsAny<TodoItem>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenTodoDoesNotExist_ShouldThrowNotFoundException()
    {
        var repositoryMock = new Mock<ITodoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var currentUserServiceMock = new Mock<ICurrentUserService>();
        var loggerMock = new Mock<ILogger<TodoService>>();

        currentUserServiceMock
            .Setup(x => x.GetRequiredUserId())
            .Returns(99);

        repositoryMock
            .Setup(x => x.GetTrackedByIdAsync(42, 99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TodoItem?)null);

        var service = new TodoService(
            repositoryMock.Object,
            unitOfWorkMock.Object,
            currentUserServiceMock.Object,
            new FixedTimeProvider(DateTimeOffset.UtcNow),
            loggerMock.Object);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            service.UpdateAsync(42, new UpdateTodoCommand("Title", "Description", true)));

        unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_Should_RemoveTrackedTodo_AndSaveChanges()
    {
        var todo = TodoItem.Create("Todo", "Description", new DateTime(2026, 3, 22, 12, 0, 0, DateTimeKind.Utc), 7);
        var repositoryMock = new Mock<ITodoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var currentUserServiceMock = new Mock<ICurrentUserService>();
        var loggerMock = new Mock<ILogger<TodoService>>();

        currentUserServiceMock
            .Setup(x => x.GetRequiredUserId())
            .Returns(7);

        repositoryMock
            .Setup(x => x.GetTrackedByIdAsync(7, 7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(todo);

        unitOfWorkMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        var service = new TodoService(
            repositoryMock.Object,
            unitOfWorkMock.Object,
            currentUserServiceMock.Object,
            new FixedTimeProvider(DateTimeOffset.UtcNow),
            loggerMock.Object);

        await service.DeleteAsync(7);

        repositoryMock.Verify(x => x.Remove(todo), Times.Once);
        unitOfWorkMock.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenTodoBelongsToAnotherUser_ShouldThrowForbiddenException()
    {
        var repositoryMock = new Mock<ITodoRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var currentUserServiceMock = new Mock<ICurrentUserService>();
        var loggerMock = new Mock<ILogger<TodoService>>();

        currentUserServiceMock
            .Setup(x => x.GetRequiredUserId())
            .Returns(10);

        repositoryMock
            .Setup(x => x.GetByIdAsync(5, 10, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TodoItem?)null);

        repositoryMock
            .Setup(x => x.ExistsAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var service = new TodoService(
            repositoryMock.Object,
            unitOfWorkMock.Object,
            currentUserServiceMock.Object,
            new FixedTimeProvider(DateTimeOffset.UtcNow),
            loggerMock.Object);

        await Assert.ThrowsAsync<ForbiddenException>(() => service.GetByIdAsync(5));
    }

    private sealed class FixedTimeProvider(DateTimeOffset utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => utcNow;
    }
}
