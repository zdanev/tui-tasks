using System;
using System.Data;
using System.Threading.Tasks;
using Terminal.Gui;

namespace TuiTasks
{
    class Program
    {
        static void Main(string[] args)
        {
            var tasksService = new TasksServiceWrapper();
            
            Application.Init();
            var top = Application.Top;

            var win = new Window("TUI Tasks")
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            var table = new TableView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };
            win.Add(table);
            top.Add(win);

            Task.Run(async () =>
            {
                // var tasksService = new TasksServiceWrapper();
                var tasks = await tasksService.ListTasks();
                Application.MainLoop.Invoke(() =>
                {
                    var dataTable = new DataTable();
                    dataTable.Columns.Add("Title");
                    dataTable.Columns.Add("Due Date");

                    foreach (var task in tasks)
                    {
                        dataTable.Rows.Add(task.Title, task.Due.HasValue ? task.Due.Value.ToShortDateString() : "");
                    }
                    table.Table = dataTable;
                });
            });

            Application.Run();
        }
    }
}
