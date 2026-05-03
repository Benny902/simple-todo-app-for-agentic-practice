namespace SimpleTaskBackend.Models.Responses;

public class TaskResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public string Priority { get; set; } = "medium";
    public DateTime CreatedAt { get; set; }
}
