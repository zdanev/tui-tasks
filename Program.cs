
using System.Threading.Tasks;

namespace TuiTasks
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var tasksService = new TasksServiceWrapper();
            var tasks = await tasksService.ListTasks();

            System.Console.WriteLine("Tasks:");
            foreach (var task in tasks)
            {
                System.Console.WriteLine($"  - {task.Title} ({task.Id}) from list {task.ListId}");
            }
        }
    }
}
