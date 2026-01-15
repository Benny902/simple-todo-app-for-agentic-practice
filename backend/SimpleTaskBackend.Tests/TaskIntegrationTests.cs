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
}
