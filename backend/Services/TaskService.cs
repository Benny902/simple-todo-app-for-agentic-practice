using Microsoft.EntityFrameworkCore;
using SimpleTaskBackend.Data;
using DbTask = SimpleTaskBackend.Models.Db.Task;

namespace SimpleTaskBackend.Services;

public class TaskService : ITaskService
{
    private readonly AppDbContext _db;
    private readonly ILogger<TaskService> _logger;

    public TaskService(AppDbContext db, ILogger<TaskService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<List<DbTask>> GetAllAsync()
    {
        _logger.LogInformation("Getting all tasks");
        return await _db.Tasks.ToListAsync();
    }

    public async Task<DbTask> CreateAsync(DbTask task)
    {
        _logger.LogInformation("Creating new task with title: {Title}", task.Title);
        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Created task with ID: {Id}", task.Id);
        return task;
    }

    public async Task<DbTask?> UpdateAsync(Guid id, string? title, bool? isCompleted)
    {
        _logger.LogInformation("Updating task {Id}. New Title: {Title}, New IsCompleted: {IsCompleted}", id, title, isCompleted);
        var task = await _db.Tasks.FindAsync(id);

        if (task is null)
        {
            _logger.LogWarning("Task {Id} not found for update", id);
            return null;
        }

        if (isCompleted.HasValue)
        {
            task.IsCompleted = isCompleted.Value;
        }
        
        if (!string.IsNullOrWhiteSpace(title))
        {
            task.Title = title;
        }

        await _db.SaveChangesAsync();
        _logger.LogInformation("Task {Id} updated successfully", id);
        return task;
    }
}
