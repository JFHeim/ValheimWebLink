using BepInEx.Logging;
using HarmonyLib;
using static ValheimWebLink.Web.Controllers.ExecuteCommand;
using Logger = BepInEx.Logging.Logger;

namespace ValheimWebLink.Web.Controllers;

[Controller]
public class ExecuteCommand : IController
{
    internal static bool runningCommand;
    internal static readonly StringBuilder commandLogBuilder = new();
    internal static bool executionHadError;
    public string Route => "/execute";
    public string HttpMethod => "POST";

    public string Description =>
        "Execute known ingame terminal command. Requires authentication. Returns logs of command execution.";

    public List<QueryParamInfo> QueryParameters => [new("command", "string", "Command to execute")];
    public bool RequiresAuth => true;

    public Task HandleRequest(HttpListenerRequest request, HttpListenerResponse response,
        Dictionary<string, string> queryParameters)
    {
        runningCommand = false;
        executionHadError = false;
        commandLogBuilder.Clear();

        string text = queryParameters.TryGetValue("command", out var command) ? command : string.Empty;
        if (text == string.Empty)
        {
            WebApiManager.SendResponce(response, BadRequest, "No command specified");
            return Task.CompletedTask;
        }

        string[] strArray = text.Split(' ');
        if (Terminal.commands.TryGetValue(strArray[0].ToLower(), out var consoleCommand))
        {
            // if (consoleCommand.IsValid(Console.instance, true))
            // {
            //     runningCommand = true;
            consoleCommand.RunAction(new(text, Console.instance));
            //     runningCommand = false;
            // } else
            // {
            //     var message = $"'{text.Split(' ')[0]}' is not valid in the current context.";
            //     WebApiManager.SendResponce(response, BadRequest, message);
            //     return Task.CompletedTask;
            // }
        } else
        {
            WebApiManager.SendResponce(response, BadRequest, "Command not found");
            return Task.CompletedTask;
        }

        string commandLog = commandLogBuilder.ToString();
        WebApiManager.SendResponce(response, executionHadError ? 500 : 200, commandLog);
        return Task.CompletedTask;
    }
}

[HarmonyPatch, HarmonyWrapSafe]
file static class Patch
{
    [HarmonyPatch(typeof(Logger), nameof(Logger.InternalLogEvent))]
    [HarmonyPrefix]
    private static bool SupressLogging(LogEventArgs eventArgs)
    {
        if (!runningCommand) return true;
        if (eventArgs.Level == LogLevel.Error || eventArgs.Level == LogLevel.Fatal) executionHadError = true;
        var value = $"[{eventArgs.Source.SourceName}:{eventArgs.Level}] {eventArgs.Data}";
        commandLogBuilder.AppendLine(value);
        return false;
    }

    [HarmonyPatch(typeof(Terminal), nameof(Terminal.AddString), [typeof(string)])]
    [HarmonyPrefix]
    private static bool SupressLoggingTerminal(string text)
    {
        if (!runningCommand) return true;
        if (text.ToLower().Contains("error")) executionHadError = true;
        var value = $"[InGameTerminal] {text}";
        commandLogBuilder.AppendLine(value);
        return false;
    }
}