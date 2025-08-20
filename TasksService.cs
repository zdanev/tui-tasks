
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Tasks.v1;
using Google.Apis.Util.Store;

namespace TuiTasks
{
    public class TasksServiceWrapper
    {
        private static readonly string[] Scopes = { TasksService.Scope.Tasks };
        private static readonly string ApplicationName = "Google Tasks API .NET Quickstart";

        private readonly TasksService _service;

        public TasksServiceWrapper()
        {
            UserCredential credential;

            using (var stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                // Console.WriteLine("Credential file saved to: " + credPath);
            }

            _service = new TasksService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        public async Task<List<GTask>> ListTasks()
        {
            var allTasks = new List<GTask>();
            TasklistsResource.ListRequest listRequest = _service.Tasklists.List();
            listRequest.MaxResults = 10;

            var taskLists = await listRequest.ExecuteAsync();
            if (taskLists.Items != null && taskLists.Items.Count > 0)
            {
                foreach (var taskList in taskLists.Items)
                {
                    TasksResource.ListRequest tasksRequest = _service.Tasks.List(taskList.Id);
                    var tasks = await tasksRequest.ExecuteAsync();
                    if (tasks.Items != null && tasks.Items.Count > 0)
                    {
                        foreach (var task in tasks.Items)
                        {
                            allTasks.Add(new GTask { Title = task.Title, Id = task.Id, ListId = taskList.Id, Due = DateTime.TryParse(task.Due, out DateTime dueDateTime) ? dueDateTime : (DateTime?)null });
                        }
                    }
                }
            }
            return allTasks;
        }

        public async Task AddTask(string taskTitle, string listId = "@default")
        {
            var task = new Google.Apis.Tasks.v1.Data.Task { Title = taskTitle };
            await _service.Tasks.Insert(task, listId).ExecuteAsync();
        }

        public async Task DeleteTask(string taskId)
        {
            var allTasks = await ListTasks();
            var taskToDelete = allTasks.FirstOrDefault(t => t.Id == taskId);

            if (taskToDelete != null)
            {
                await _service.Tasks.Delete(taskToDelete.ListId, taskId).ExecuteAsync();
                // Console.WriteLine($"Task '{taskToDelete.Title}' deleted.");
            }
            else
            {
                // Console.WriteLine("Task not found.");
            }
        }
    }
}
