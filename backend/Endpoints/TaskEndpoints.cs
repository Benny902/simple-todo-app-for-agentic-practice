using backend.Data;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Endpoints;

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

        group.MapPost("/", async (backend.Models.Task task, AppDbContext db) =>
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
    }
}
