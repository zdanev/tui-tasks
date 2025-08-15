# TUI Tasks

This is a simple console application that uses the Google Tasks API to list your task lists and tasks.

## Setup

1.  **Enable the Google Tasks API and download your client secret.**

    1.  Go to the [Google Cloud Console](https://console.cloud.google.com/).
    2.  Create a new project or select an existing one.
    3.  In the navigation menu, go to **APIs & Services** > **Enabled APIs & services**.
    4.  Click **+ ENABLE APIS AND SERVICES**, search for "Google Tasks API", and enable it.
    5.  In the navigation menu, go to **APIs & Services** > **Credentials**.
    6.  Click **+ CREATE CREDENTIALS** > **OAuth client ID**.
    7.  Select **Desktop app** as the application type.
    8.  Click **Create**.
    9.  In the list of OAuth 2.0 Client IDs, find your newly created client ID and click the download icon to download the `client_secret.json` file.
    10. Place the downloaded `client_secret.json` file in the `bin/Debug/net8.0` directory of this project.

2.  **Run the application.**

    To list your tasks, run the application without any parameters:

    ```bash
    dotnet run
    ```

    To add a new task, use the `-a` parameter followed by the task description:

    ```bash
    dotnet run -- -a "Your new task description"
    ```
