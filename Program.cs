
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
            else
            {
                var tasks = await tasksService.ListTasks();

                System.Console.WriteLine("Tasks:");
                foreach (var task in tasks)
                {
                    System.Console.WriteLine($"  - {task.Title} ({task.Id}) from list {task.ListId}");
                }
            }
        }
    }
}
