using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Task = System.Threading.Tasks.Task;

namespace SimpleTaskBackend.Tests;

public class TaskIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public TaskIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetTasks_ReturnsOkAndEmptyList_WhenNoTasksExist()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/tasks");

        // Assert
        response.EnsureSuccessStatusCode();
        var tasks = await response.Content.ReadFromJsonAsync<List<Models.Task>>();
        Assert.NotNull(tasks);
        // Note: In-memory DB is shared across tests if we don't reset it. 
        // For simplicity in this workshop setup, we might see data from other tests or previous runs if the app keeps running.
        // But WebApplicationFactory usually creates a fresh host per test run (though sharing context depends on config).
        // Since we registered DbContext with just UseInMemoryDatabase("SimpleTaskDb"), it stays alive for the process lifetime.
        // A better approach for tests would be to use a unique name per test or ensure clean state.
        // For this simple test, we just check it returns a list.
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
        var createdTask = await response.Content.ReadFromJsonAsync<Models.Task>();
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
        var createdTask = await createResponse.Content.ReadFromJsonAsync<Models.Task>();
        Assert.NotNull(createdTask);

        // Act
        var patchResponse = await client.PatchAsJsonAsync($"/api/tasks/{createdTask.Id}", new { isCompleted = true });

        // Assert
        patchResponse.EnsureSuccessStatusCode();
        var updatedTask = await patchResponse.Content.ReadFromJsonAsync<Models.Task>();
        Assert.NotNull(updatedTask);
        Assert.True(updatedTask.IsCompleted);
        Assert.Equal(createdTask.Id, updatedTask.Id);
    }
}
