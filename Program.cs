using TaskTracker.Services;

var service = new TaskService();

Console.WriteLine("TaskTracker — type 'help' for commands");

while (true)
{
    Console.Write("> ");
    var input = Console.ReadLine()?.Trim();
    if (string.IsNullOrEmpty(input)) continue;

    var parts = input.Split(' ', 2);
    var command = parts[0].ToLower();
    var argument = parts.Length > 1 ? parts[1] : "";

    switch (command)
    {
        case "add":
            if (string.IsNullOrWhiteSpace(argument))
            {
                Console.WriteLine("Usage: add <title>");
                break;
            }
            var task = service.Add(argument);
            Console.WriteLine($"Added: {task}");
            break;

        case "list":
            var tasks = service.GetAll();
            if (tasks.Count == 0)
            {
                Console.WriteLine("No tasks.");
                break;
            }
            foreach (var t in tasks)
                Console.WriteLine(t);
            break;

        case "complete":
            if (!int.TryParse(argument, out var completeId))
            {
                Console.WriteLine("Usage: complete <id>");
                break;
            }
            Console.WriteLine(service.Complete(completeId) ? "Done." : "Task not found.");
            break;

        case "remove":
            if (!int.TryParse(argument, out var removeId))
            {
                Console.WriteLine("Usage: remove <id>");
                break;
            }
            Console.WriteLine(service.Remove(removeId) ? "Removed." : "Task not found.");
            break;

        case "stats":
            Console.WriteLine(service.GetStats());
            break;

        case "search":
            if (string.IsNullOrWhiteSpace(argument))
            {
                Console.WriteLine("Usage: search <keyword>");
                break;
            }
            var results = service.Search(argument);
            if (results.Count == 0)
                Console.WriteLine("No matches.");
            else
                foreach (var r in results)
                    Console.WriteLine(r);
            break;

        case "export":
            Console.WriteLine(service.ExportCsv());
            break;

        case "help":
            Console.WriteLine("Commands: add, list, complete, remove, stats, search, export, quit");
            break;

        case "quit":
        case "exit":
            return;

        default:
            Console.WriteLine($"Unknown command: {command}. Type 'help'.");
            break;
    }
}
