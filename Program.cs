using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Tasks.v1;
using Google.Apis.Util.Store;

namespace TuiTasks
{
    class Program
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/tasks.googleapis.com-dotnet-quickstart.json
        static string[] Scopes = { TasksService.Scope.TasksReadonly };
        static string ApplicationName = "Google Tasks API .NET Quickstart";

        static async Task Main(string[] args)
        {
            UserCredential credential;

            using (var stream =
                new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true));
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Tasks API service.
            var service = new TasksService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define parameters of request.
            TasklistsResource.ListRequest listRequest = service.Tasklists.List();
            listRequest.MaxResults = 10;

            // List task lists.
            var taskLists = await listRequest.ExecuteAsync();
            Console.WriteLine("Task Lists:");
            if (taskLists.Items != null && taskLists.Items.Count > 0)
            {
                foreach (var taskList in taskLists.Items)
                {
                    Console.WriteLine("{0} ({1})", taskList.Title, taskList.Id);

                    // Now retrieve the tasks for each task list
                    TasksResource.ListRequest tasksRequest = service.Tasks.List(taskList.Id);
                    var tasks = await tasksRequest.ExecuteAsync();
                    if (tasks.Items != null && tasks.Items.Count > 0)
                    {
                        foreach (var task in tasks.Items)
                        {
                            Console.WriteLine("  - {0} ({1})", task.Title, task.Id);
                        }
                    }
                    else
                    {
                        Console.WriteLine("  No tasks found in this list.");
                    }
                }
            }
            else
            {
                Console.WriteLine("No task lists found.");
            }
        }
    }
}
