#nullable enable
using BepInEx.Logging;
using ValheimWebLink.Web;
using static System.ConsoleColor;

namespace ValheimWebLink.ConsoleCommands;

public static class ConsoleCommandsManager
{
    private static DiskLogListener DiskDebugListener = null!;
    internal static bool isTyping = false;

    public static void Init()
    {
        foreach (var listener in BepInEx.Logging.Logger.Listeners)
            if (listener.GetType() == typeof(DiskLogListener))
                DiskDebugListener = (DiskLogListener)listener;

        if (!Application.isBatchMode)
        {
            DebugError("This script should only be used in batch mode (headless server).");
            return;
        }

        Debug(
            "\n---\n"
            + $"You are running {ModName} by {ModAuthor} version {ModVersion}\n"
            + $"You can comunicate with {ModName} via commands in this terminal\n"
            + "All commands start with 'vwl '\n"
            + "Try 'vwl help' for more info\n"
            + "If while you was typing other Debug appear, don't worry, just continue typing, it will work\n"
            + "\n---\n", DarkGreen);

        Task.Run(ListenForInput, WebApiManager._cancellationTokenSource.Token);
    }


    private static void ListenForInput()
    {
        while (true)
        {
            if (WebApiManager._cancellationTokenSource.IsCancellationRequested) break;
            try
            {
                string? input = System.Console.ReadLine();
                if (input == null) continue;
                HandleConsoleCommands(input);
            }
            catch (Exception e)
            {
                DebugError($"Couth exception: {e}");
            }
        }
    }

    private static void HandleConsoleCommands(string input)
    {
        if (!input.StartsWith("vwl"))
            return;
        // Remove vwl
        input = input.Remove(0, 4);

        if (input.Equals("help") || input.Equals("-help") || input.Equals("--help") ||
            input.Equals("h") || input.Equals("-h") || input.Equals("--h"))
        {
            Debug($"Welcome to {ModName} by {ModAuthor}. Version: {ModVersion}");
            Debug("Command example: 'vwl clear'");
            Debug("");
            Debug("Available commands:");
            Debug("clear - clear the console");
            Debug("stop - stops listening for requests");
            Debug("getport - prints the port number");
            Debug("restart - restarts the http server");
            Debug("");
            Debug("Available requests:");
            Debug("GET / - returns help info with all available requests");
            Debug("GET /serverinfo - returns lots of information about the server");
            Debug("");
            Debug("Info:");
            Debug("Discord: @justafrogger");
            Debug("GitHub: https://github.com/FroggerHH/ValheimWebLink");
            return;
        }


        if (input.Equals("stop"))
        {
            Debug("Bye!");
            return;
        }

        if (input.Equals("clear"))
        {
            System.Console.Clear();
            return;
        }

        if (input.Equals("getport"))
        {
            Debug($"Port={WebApiManager.Port}");
            return;
        }

        if (input.Equals("restart"))
        {
            WebApiManager.Stop();
            WebApiManager.Init(WebApiManager.Port);
            return;
        }

        Debug($"Unknown command: {input}");
    }
}