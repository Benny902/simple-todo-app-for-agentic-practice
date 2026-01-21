using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using SimpleTaskBackend.Data;
using SimpleTaskBackend.Models.Responses;
using Task = System.Threading.Tasks.Task;

namespace SimpleTaskBackend.Tests;

public class TaskIntegrationTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;

    public TaskIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        // Reset the database state before every test.
        // This is necessary because IClassFixture shares the WebApplicationFactory (and thus the In-Memory DB) 
        // across all tests in this class for performance.
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task GetTasks_ReturnsOkAndEmptyList_WhenNoTasksExist()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/tasks");

        // Assert
        response.EnsureSuccessStatusCode();
        var tasks = await response.Content.ReadFromJsonAsync<List<TaskResponse>>();
        Assert.NotNull(tasks);
        Assert.Empty(tasks);
    }

    [Fact]
    public async Task CreateTask_ReturnsCreatedAndTask()
    {
        // Arrange
        var client = _factory.CreateClient();
        var newTask = new { title = "Integration Test Task" };

        // Act
        var response = await client.PostAsJsonAsync("/api/tasks", newTask);

        // Assert
        response.EnsureSuccessStatusCode(); // Status Code 201-299
        var createdTask = await response.Content.ReadFromJsonAsync<TaskResponse>();
        Assert.NotNull(createdTask);
        Assert.Equal(newTask.title, createdTask.Title);
        Assert.False(createdTask.IsCompleted);
        Assert.NotEqual(Guid.Empty, createdTask.Id);
    }

    [Fact]
    public async Task PatchTask_UpdatesCompletionStatus()
    {
        // Arrange
        var client = _factory.CreateClient();
        // Create a task first
        var createResponse = await client.PostAsJsonAsync("/api/tasks", new { title = "Task to Complete" });
        createResponse.EnsureSuccessStatusCode();
        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskResponse>();
        Assert.NotNull(createdTask);

        // Act
        var patchResponse = await client.PatchAsJsonAsync($"/api/tasks/{createdTask.Id}", new { isCompleted = true });

        // Assert
        patchResponse.EnsureSuccessStatusCode();
        var updatedTask = await patchResponse.Content.ReadFromJsonAsync<TaskResponse>();
        Assert.NotNull(updatedTask);
        Assert.True(updatedTask.IsCompleted);
        Assert.Equal(createdTask.Id, updatedTask.Id);
    }

    [Fact]
    public async Task CreateTask_WithDescription_ReturnsCreatedTaskWithDescription()
    {
        // Arrange
        var client = _factory.CreateClient();
        var newTask = new { title = "Task with Description", description = "This is a detailed description" };

        // Act
        var response = await client.PostAsJsonAsync("/api/tasks", newTask);

        // Assert
        response.EnsureSuccessStatusCode();
        var createdTask = await response.Content.ReadFromJsonAsync<TaskResponse>();
        Assert.NotNull(createdTask);
        Assert.Equal(newTask.title, createdTask.Title);
        Assert.Equal(newTask.description, createdTask.Description);
        Assert.False(createdTask.IsCompleted);
    }

    [Fact]
    public async Task CreateTask_WithoutDescription_ReturnsCreatedTaskWithNullDescription()
    {
        // Arrange
        var client = _factory.CreateClient();
        var newTask = new { title = "Task without Description" };

        // Act
        var response = await client.PostAsJsonAsync("/api/tasks", newTask);

        // Assert
        response.EnsureSuccessStatusCode();
        var createdTask = await response.Content.ReadFromJsonAsync<TaskResponse>();
        Assert.NotNull(createdTask);
        Assert.Equal(newTask.title, createdTask.Title);
        Assert.Null(createdTask.Description);
    }

    [Fact]
    public async Task PatchTask_UpdatesDescription()
    {
        // Arrange
        var client = _factory.CreateClient();
        var createResponse = await client.PostAsJsonAsync("/api/tasks", new { title = "Task to Update" });
        createResponse.EnsureSuccessStatusCode();
        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskResponse>();
        Assert.NotNull(createdTask);

        // Act
        var patchResponse = await client.PatchAsJsonAsync($"/api/tasks/{createdTask.Id}", 
            new { description = "Updated description" });

        // Assert
        patchResponse.EnsureSuccessStatusCode();
        var updatedTask = await patchResponse.Content.ReadFromJsonAsync<TaskResponse>();
        Assert.NotNull(updatedTask);
        Assert.Equal("Updated description", updatedTask.Description);
        Assert.Equal(createdTask.Title, updatedTask.Title);
    }

    [Fact]
    public async Task DeleteTask_SoftDeletesTask()
    {
        // Arrange
        var client = _factory.CreateClient();
        var createResponse = await client.PostAsJsonAsync("/api/tasks", new { title = "Task to Delete" });
        createResponse.EnsureSuccessStatusCode();
        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskResponse>();
        Assert.NotNull(createdTask);

        // Act
        var deleteResponse = await client.DeleteAsync($"/api/tasks/{createdTask.Id}");

        // Assert
        deleteResponse.EnsureSuccessStatusCode();
        Assert.Equal(System.Net.HttpStatusCode.NoContent, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task GetTasks_DoesNotReturnDeletedTasks()
    {
        // Arrange
        var client = _factory.CreateClient();
        
        // Create two tasks
        var createResponse1 = await client.PostAsJsonAsync("/api/tasks", new { title = "Task 1" });
        createResponse1.EnsureSuccessStatusCode();
        var task1 = await createResponse1.Content.ReadFromJsonAsync<TaskResponse>();
        Assert.NotNull(task1);

        var createResponse2 = await client.PostAsJsonAsync("/api/tasks", new { title = "Task 2" });
        createResponse2.EnsureSuccessStatusCode();
        var task2 = await createResponse2.Content.ReadFromJsonAsync<TaskResponse>();
        Assert.NotNull(task2);

        // Delete first task
        var deleteResponse = await client.DeleteAsync($"/api/tasks/{task1.Id}");
        deleteResponse.EnsureSuccessStatusCode();

        // Act
        var getResponse = await client.GetAsync("/api/tasks");

        // Assert
        getResponse.EnsureSuccessStatusCode();
        var tasks = await getResponse.Content.ReadFromJsonAsync<List<TaskResponse>>();
        Assert.NotNull(tasks);
        Assert.Single(tasks);
        Assert.Equal(task2.Id, tasks[0].Id);
        Assert.DoesNotContain(tasks, t => t.Id == task1.Id);
    }

    [Fact]
    public async Task DeleteTask_TaskRemainsInDatabase()
    {
        // Arrange
        var client = _factory.CreateClient();
        var createResponse = await client.PostAsJsonAsync("/api/tasks", new { title = "Task to Soft Delete" });
        createResponse.EnsureSuccessStatusCode();
        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskResponse>();
        Assert.NotNull(createdTask);

        // Act - Delete the task
        var deleteResponse = await client.DeleteAsync($"/api/tasks/{createdTask.Id}");
        deleteResponse.EnsureSuccessStatusCode();

        // Assert - Check that task still exists in the database but is marked as deleted
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var taskInDb = await db.Tasks.FindAsync(createdTask.Id);
        Assert.NotNull(taskInDb);
        Assert.True(taskInDb.IsDeleted);
    }

    [Fact]
    public async Task UpdateTask_CannotUpdateDeletedTask()
    {
        // Arrange
        var client = _factory.CreateClient();
        var createResponse = await client.PostAsJsonAsync("/api/tasks", new { title = "Task to Delete and Update" });
        createResponse.EnsureSuccessStatusCode();
        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskResponse>();
        Assert.NotNull(createdTask);

        // Delete the task
        var deleteResponse = await client.DeleteAsync($"/api/tasks/{createdTask.Id}");
        deleteResponse.EnsureSuccessStatusCode();

        // Act - Try to update the deleted task
        var updateResponse = await client.PatchAsJsonAsync($"/api/tasks/{createdTask.Id}", 
            new { title = "Updated Title" });

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, updateResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteTask_NonExistentTask_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();
        var nonExistentId = Guid.NewGuid();

        // Act
        var deleteResponse = await client.DeleteAsync($"/api/tasks/{nonExistentId}");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, deleteResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteTask_AlreadyDeletedTask_ReturnsNotFound()
    {
        // Arrange
        var client = _factory.CreateClient();
        var createResponse = await client.PostAsJsonAsync("/api/tasks", new { title = "Task to Delete Twice" });
        createResponse.EnsureSuccessStatusCode();
        var createdTask = await createResponse.Content.ReadFromJsonAsync<TaskResponse>();
        Assert.NotNull(createdTask);

        // Delete the task once
        var firstDeleteResponse = await client.DeleteAsync($"/api/tasks/{createdTask.Id}");
        firstDeleteResponse.EnsureSuccessStatusCode();

        // Act - Try to delete again
        var secondDeleteResponse = await client.DeleteAsync($"/api/tasks/{createdTask.Id}");

        // Assert
        Assert.Equal(System.Net.HttpStatusCode.NotFound, secondDeleteResponse.StatusCode);
    }
}
