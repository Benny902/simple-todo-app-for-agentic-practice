using System.ComponentModel.DataAnnotations;

namespace SimpleTaskBackend.Models.Requests;

public class CreateTaskRequest
{
    [Required]
    [MinLength(1)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [RegularExpression("^(low|medium|high)$")]
    public string Priority { get; set; } = "medium";
}
