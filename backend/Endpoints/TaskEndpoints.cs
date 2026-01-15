using Microsoft.EntityFrameworkCore;
using SimpleTaskBackend.Data;
using Task = SimpleTaskBackend.Models.Task;

namespace SimpleTaskBackend.Endpoints;

public record UpdateTaskRequest(string? Title, bool? IsCompleted);

public static class TaskEndpoints
{
    public static void MapTaskEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/tasks");

        group.MapGet("/", async (AppDbContext db) =>
        {
            return await db.Tasks.ToListAsync();
        })
        .WithName("GetTasks");

        group.MapPost("/", async (Task task, AppDbContext db) =>
        {
            if (string.IsNullOrWhiteSpace(task.Title))
            {
                return Results.BadRequest("Title is required");
            }

            db.Tasks.Add(task);
            await db.SaveChangesAsync();
            return Results.Created($"/api/tasks/{task.Id}", task);
        })
        .WithName("CreateTask");

        group.MapPatch("/{id:guid}", async (Guid id, UpdateTaskRequest request, AppDbContext db) =>
        {
            var task = await db.Tasks.FindAsync(id);

            if (task is null) return Results.NotFound();

            if (request.IsCompleted.HasValue)
            {
                task.IsCompleted = request.IsCompleted.Value;
            }
            
            if (!string.IsNullOrWhiteSpace(request.Title))
            {
                task.Title = request.Title;
            }

            await db.SaveChangesAsync();

            return Results.Ok(task);
        })
        .WithName("UpdateTask");
    }
}
