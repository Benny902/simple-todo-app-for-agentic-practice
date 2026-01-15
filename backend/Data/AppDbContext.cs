using Microsoft.EntityFrameworkCore;
using Task = SimpleTaskBackend.Models.Db.Task;

namespace SimpleTaskBackend.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Task> Tasks { get; set; }
}
