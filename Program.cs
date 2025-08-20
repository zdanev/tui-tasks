using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Terminal.Gui;

namespace TuiTasks
{
    class Program
    {
        private static List<GTask> tasks = new List<GTask>();
        private static TasksServiceWrapper tasksService = new TasksServiceWrapper();
        private static TableView? table;

        static void Main(string[] args)
        {
            Application.Init();
            var top = Application.Top;

            var win = new Window("TUI Tasks")
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill() - 1
            };

            table = new TableView()
            {
                X = 0,
                Y = 0,
                Width = Dim.Fill(),
                Height = Dim.Fill(),
                FullRowSelect = true
            };
            win.Add(table);

            var statusBar = new StatusBar(new StatusItem[] {
                new StatusItem(Key.CtrlMask | Key.N, "~^N~ Add Task", AddTask),
                new StatusItem(Key.CtrlMask | Key.D, "~^D~ Delete Task", DeleteTask),
            });

            top.Add(win, statusBar);

            RefreshTasks();

            Application.Run();
        }

        private static void RefreshTasks()
        {
            Task.Run(async () =>
            {
                tasks = await tasksService.ListTasks();
                Application.MainLoop.Invoke(() =>
                {
                    if (table != null)
                    {
                        var dataTable = new DataTable();
                        var dueDateColumn = new DataColumn("Due Date");
                        var titleColumn = new DataColumn("Title");
                        dataTable.Columns.Add(dueDateColumn);
                        dataTable.Columns.Add(titleColumn);

                        foreach (var task in tasks)
                        {
                            dataTable.Rows.Add(task.Due.HasValue ? task.Due.Value.ToShortDateString() : "", task.Title);
                        }
                        table.Table = dataTable;
                        table.Style.ColumnStyles.Add(dueDateColumn, new Terminal.Gui.TableView.ColumnStyle() { MaxWidth = 12 });
                    }
                });
            });
        }

        private static void AddTask()
        {
            var dialog = new Dialog("Add Task", 60, 7);
            var titleLabel = new Label("Title:") { X = 1, Y = 1 };
            var titleText = new TextField("") { X = 10, Y = 1, Width = 40 };
            var okButton = new Button("OK");
            var cancelButton = new Button("Cancel");

            okButton.Clicked += () => {
                var taskTitle = titleText.Text?.ToString();
                if (!string.IsNullOrEmpty(taskTitle))
                {
                    Task.Run(async () =>
                    {
                        await tasksService.AddTask(taskTitle);
                        RefreshTasks();
                    });
                    Application.RequestStop();
                }
            };
            cancelButton.Clicked += () => { Application.RequestStop(); };

            dialog.Add(titleLabel, titleText);
            dialog.AddButton(okButton);
            dialog.AddButton(cancelButton);
            Application.Run(dialog);
        }

        private static void DeleteTask()
        {
            if (table != null && table.SelectedRow >= 0 && table.SelectedRow < tasks.Count)
            {
                var selectedTask = tasks[table.SelectedRow];
                if (selectedTask.Id != null)
                {
                    var dialog = new Dialog("Delete Task", 60, 7);
                    var message = new Label($"Are you sure you want to delete '{selectedTask.Title}'?") { X = 1, Y = 1 };
                    var okButton = new Button("OK");
                    var cancelButton = new Button("Cancel");

                    okButton.Clicked += () => {
                        Task.Run(async () =>
                        {
                            await tasksService.DeleteTask(selectedTask.Id);
                            RefreshTasks();
                        });
                        Application.RequestStop();
                    };
                    cancelButton.Clicked += () => { Application.RequestStop(); };

                    dialog.Add(message);
                    dialog.AddButton(okButton);
                    dialog.AddButton(cancelButton);
                    Application.Run(dialog);
                }
            }
        }
    }
}
