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
                X = -1,
                Y = -1,
                Width = Dim.Fill() + 1,
                Height = Dim.Fill() + 1,
                FullRowSelect = true
            };
            table.Style.ExpandLastColumn = true;
            win.Add(table);
            top.Add(win);

            Task.Run(async () =>
            {
                var tasksService = new TasksServiceWrapper();
                var tasks = await tasksService.ListTasks();
                Application.MainLoop.Invoke(() =>
                {
                    var dataTable = new DataTable();
                    var titleColumn = new DataColumn("Title");
                    var dueDateColumn = new DataColumn("Due Date");
                    dataTable.Columns.Add(dueDateColumn);
                    dataTable.Columns.Add(titleColumn);

                    foreach (var task in tasks)
                    {
                        dataTable.Rows.Add(
                            task.Due.HasValue ? task.Due.Value.ToShortDateString() : "",
                            task.Title);
                    }
                    table.Table = dataTable;
                
                    // table.Style.ColumnStyles.Add(titleColumn, new ColumnStyles() { });
                    table.Style.ColumnStyles.Add(dueDateColumn, new Terminal.Gui.TableView.ColumnStyle() { MaxWidth = 12 });
                });
            });

            Application.Run();
        }
    }
}
