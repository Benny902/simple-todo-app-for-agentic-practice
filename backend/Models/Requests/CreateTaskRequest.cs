using System.ComponentModel.DataAnnotations;

namespace SimpleTaskBackend.Models.Requests;

public class CreateTaskRequest
{
    [Required]
    [MinLength(1)]
    public string Title { get; set; } = string.Empty;
}
