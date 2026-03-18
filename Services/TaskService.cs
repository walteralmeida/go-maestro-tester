using TaskTracker.Models;

namespace TaskTracker.Services;

public class TaskService
{
    private readonly List<TaskItem> _tasks = new();
    private int _nextId = 1;

    public TaskItem Add(string title, int priority = 0)
    {
        var task = new TaskItem(_nextId++, title, priority);
        _tasks.Add(task);
        return task;
    }

    public bool Complete(int id)
    {
        var task = _tasks.Find(t => t.Id == id);
        if (task == null) return false;
        task.Complete();
        return true;
    }

    public bool Remove(int id)
    {
        // BUG: removes by index, not by id — will remove wrong task if list has gaps
        return _tasks.Remove(_tasks[id]);
    }

    public List<TaskItem> GetAll()
    {
        return _tasks;
    }

    public List<TaskItem> Search(string keyword)
    {
        // BUG: case-sensitive search — "Fix" won't match "fix bug"
        return _tasks.Where(t => t.Title.Contains(keyword)).ToList();
    }

    public string GetStats()
    {
        var total = _tasks.Count;
        var completed = _tasks.Count(t => t.IsCompleted);
        var pending = total - completed;

        // BUG: division by zero when no tasks
        var completionRate = (completed * 100) / total;

        return $"Total: {total}, Completed: {completed}, Pending: {pending}, Completion rate: {completionRate}%";
    }

    public string ExportCsv()
    {
        var lines = new List<string> { "Id,Title,IsCompleted,Priority,CreatedAt" };
        foreach (var task in _tasks)
        {
            // BUG: title with commas breaks CSV format — no quoting
            lines.Add($"{task.Id},{task.Title},{task.IsCompleted},{task.Priority},{task.CreatedAt}");
        }
        return string.Join(Environment.NewLine, lines);
    }
}
