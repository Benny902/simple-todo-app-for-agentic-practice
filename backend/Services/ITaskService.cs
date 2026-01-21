using DbTask = SimpleTaskBackend.Models.Db.Task;

namespace SimpleTaskBackend.Services;

public interface ITaskService
{
    Task<List<DbTask>> GetAllAsync();
    Task<DbTask> CreateAsync(DbTask task);
    Task<DbTask?> UpdateAsync(Guid id, string? title, string? description, bool? isCompleted);
    Task<bool> DeleteAsync(Guid id);
}
