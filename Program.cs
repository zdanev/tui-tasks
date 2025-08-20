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
                X = -1,
                Y = -1,
                Width = Dim.Fill() + 2,
                Height = Dim.Fill() + 2,
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
                            dataTable.Rows.Add(task.Due.HasValue ? task.Due.Value.ToString("d") : "", task.Title);
                        }
                        table.Table = dataTable;
                        table.Style.ColumnStyles.Add(dueDateColumn, new Terminal.Gui.TableView.ColumnStyle() { MaxWidth = 12 });
                    }
                });
            });
        }

        private static void AddTask()
        {
            var dialog = new Dialog("Add Task", 60, 11);
            var titleLabel = new Label("Title:") { X = 1, Y = 1 };
            var titleText = new TextField("") { X = 10, Y = 1, Width = 40 };
            var dueDateLabel = new Label("Due Date:") { X = 1, Y = 3 };
            var dateField = new DateField(DateTime.Now) { X = 10, Y = 3, Width = 40 };
            var dueTimeLabel = new Label("Due Time:") { X = 1, Y = 5 };
            var timeField = new TimeField(DateTime.Now.TimeOfDay) { X = 10, Y = 5, Width = 40 };

            var okButton = new Button("OK");
            var cancelButton = new Button("Cancel");

            okButton.Clicked += () => {
                var taskTitle = titleText.Text?.ToString();
                if (!string.IsNullOrEmpty(taskTitle))
                {
                    var date = dateField.Date;
                    var time = timeField.Time;
                    var dateTime = date.Add(time);
                    var dateTimeOffset = new DateTimeOffset(dateTime, TimeZoneInfo.Local.GetUtcOffset(dateTime));

                    Task.Run(async () =>
                    {
                        await tasksService.AddTask(taskTitle, dateTimeOffset);
                        RefreshTasks();
                    });
                    Application.RequestStop();
                }
            };
            cancelButton.Clicked += () => { Application.RequestStop(); };

            dialog.Add(titleLabel, titleText, dueDateLabel, dateField, dueTimeLabel, timeField);
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
