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
        var tasks = await _db.Tasks.ToListAsync();

        return tasks
            .OrderBy(t => t.IsCompleted)
            .ThenBy(t => GetPriorityRank(t.Priority))
            .ThenBy(t => t.CreatedAt)
            .ToList();
    }

    public async Task<DbTask> CreateAsync(DbTask task)
    {
        task.Priority = NormalizePriority(task.Priority);
        _logger.LogInformation("Creating new task with title: {Title}, priority: {Priority}", task.Title, task.Priority);
        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();
        _logger.LogInformation("Created task with ID: {Id}", task.Id);
        return task;
    }

    public async Task<DbTask?> UpdateAsync(Guid id, string? title, bool? isCompleted, string? priority)
    {
        _logger.LogInformation(
            "Updating task {Id}. New Title: {Title}, New IsCompleted: {IsCompleted}, New Priority: {Priority}",
            id,
            title,
            isCompleted,
            priority
        );
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

        if (!string.IsNullOrWhiteSpace(priority))
        {
            task.Priority = NormalizePriority(priority);
        }

        await _db.SaveChangesAsync();
        _logger.LogInformation("Task {Id} updated successfully", id);
        return task;
    }

    private static string NormalizePriority(string? priority)
    {
        return string.IsNullOrWhiteSpace(priority) ? "medium" : priority.Trim().ToLowerInvariant();
    }

    private static int GetPriorityRank(string? priority)
    {
        return priority?.ToLowerInvariant() switch
        {
            "high" => 0,
            "medium" => 1,
            "low" => 2,
            _ => 1,
        };
    }
}
