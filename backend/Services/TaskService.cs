using Microsoft.EntityFrameworkCore;
using SimpleTaskBackend.Data;
using DbTask = SimpleTaskBackend.Models.Db.Task;

namespace SimpleTaskBackend.Services;

public class TaskService : ITaskService
{
    private readonly AppDbContext _db;

    public TaskService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<DbTask>> GetAllAsync()
    {
        return await _db.Tasks.ToListAsync();
    }

    public async Task<DbTask> CreateAsync(DbTask task)
    {
        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();
        return task;
    }

    public async Task<DbTask?> UpdateAsync(Guid id, string? title, bool? isCompleted)
    {
        var task = await _db.Tasks.FindAsync(id);

        if (task is null) return null;

        if (isCompleted.HasValue)
        {
            task.IsCompleted = isCompleted.Value;
        }
        
        if (!string.IsNullOrWhiteSpace(title))
        {
            task.Title = title;
        }

        await _db.SaveChangesAsync();
        return task;
    }
}
