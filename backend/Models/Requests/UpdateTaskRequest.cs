using System.ComponentModel.DataAnnotations;

namespace SimpleTaskBackend.Models.Requests;

public class UpdateTaskRequest
{
    [MinLength(1)]
    public string? Title { get; set; }
    
    public string? Description { get; set; }
    
    public bool? IsCompleted { get; set; }
}
