using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Task = System.Threading.Tasks.Task;

namespace SimpleTaskBackend.Tests;

public class TaskValidationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public TaskValidationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateTask_ReturnsBadRequest_WhenTitleIsEmpty()
    {
        // Arrange
        var client = _factory.CreateClient();
        var invalidTask = new { title = "" };

        // Act
        var response = await client.PostAsJsonAsync("/api/tasks", invalidTask);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        
        // Optional: verify validation problem details
        var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(problemDetails);
        Assert.Contains("Title", problemDetails.Errors.Keys);
    }
}
