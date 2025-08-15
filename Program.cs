
using System.Threading.Tasks;

namespace TuiTasks
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var tasksService = new TasksServiceWrapper();

            if (args.Length > 1 && args[0] == "-a")
            {
                await tasksService.AddTask(args[1]);
                System.Console.WriteLine("Task added.");
            }
            else if (args.Length > 1 && args[0] == "-d")
            {
                await tasksService.DeleteTask(args[1]);
            }
            else
            {
                var tasks = await tasksService.ListTasks();

                System.Console.WriteLine("Tasks:");
                foreach (var task in tasks)
                {
                  string due = task.Due.HasValue ? "[" + task.Due.Value.ToShortDateString() + "] " : "";
                    System.Console.WriteLine($"{due}{task.Title} ({task.Id})");
                }
            }
        }
    }
}
