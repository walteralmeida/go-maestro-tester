namespace TaskTracker.Models;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    // BUG: Priority is an int but never validated — accepts negative values
    public int Priority { get; set; }

    public TaskItem(int id, string title, int priority = 0)
    {
        Id = id;
        Title = title;
        IsCompleted = false;
        CreatedAt = DateTime.Now;
        Priority = priority;
    }

    public void Complete()
    {
        IsCompleted = true;
        // BUG: CompletedAt is never set
    }

    public override string ToString()
    {
        var status = IsCompleted ? "✓" : " ";
        return $"[{status}] #{Id} {Title} (priority: {Priority})";
    }
}
