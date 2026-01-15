using System.ComponentModel.DataAnnotations;

namespace SimpleTaskBackend.Models.Db;

public class Task
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [Required]
    [MinLength(1)]
    public required string Title { get; set; }
    
    public bool IsCompleted { get; set; } = false;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
