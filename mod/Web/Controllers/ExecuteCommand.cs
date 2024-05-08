using System.IO;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine.Serialization;
using static ValheimWebLink.Web.Controllers.ExecuteCommand;

namespace ValheimWebLink.Web.Controllers;

[Controller]
public class ExecuteCommand : IController
{
    internal static bool runningCommand = false;
    internal static StringBuilder commandLogBuilder = new();
    internal static bool executionHadError = false;
    public string Route => "/execute";
    public string HttpMethod => "POST";
    public string Description => "Execute known ingame terminal command";

    public void HandleRequest(HttpListenerRequest request, HttpListenerResponse response, bool isAuthed,
        Dictionary<string, string> queryParameters)
    {
        runningCommand = false;
        executionHadError = false;
        commandLogBuilder.Clear();

        if (!isAuthed)
        {
            response.StatusCode = 401;
            response.ContentType = "application/json";
            response.ContentEncoding = Encoding.UTF8;
            response.KeepAlive = false;
            response.Close();
            return;
        }

        ErrorResult? error = null;
        string text = queryParameters.TryGetValue("command", out var command) ? command : string.Empty;
        if (text == string.Empty)
        {
            error = new("No command specified");

            response.StatusCode = 400;
            response.ContentType = "application/json";
            response.ContentEncoding = Encoding.UTF8;
            string responseString = JSON.ToJSON(error);
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            using Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);

            return;
        }

        string[] strArray = text.Split(' ');
        if (Terminal.commands.TryGetValue(strArray[0].ToLower(), out var consoleCommand))
        {
            if (consoleCommand.IsValid(Console.instance))
            {
                runningCommand = true;
                consoleCommand.RunAction(new(text, Console.instance));
                runningCommand = false;
            } else error = new($"'{text.Split(' ')[0]}' is not valid in the current context.");
        } else
        {
            error = new("Command not found");
        }

        string commandLog = commandLogBuilder.ToString();

        if (error.HasValue)
        {
            response.StatusCode = executionHadError ? 500 : 400;
            response.ContentType = "application/json";
            response.ContentEncoding = Encoding.UTF8;
            string responseString = JSON.ToJSON(error);
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);
            response.ContentLength64 = buffer.Length;
            using Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            return;
        }

        response.StatusCode = executionHadError ? 500 : 200;
        response.ContentType = "text/plain";
        response.ContentEncoding = Encoding.UTF8;
        string _responseString = commandLog.IsGood() ? commandLog : "No log";
        byte[] _buffer = Encoding.UTF8.GetBytes(_responseString);
        response.ContentLength64 = _buffer.Length;
        using Stream _output = response.OutputStream;
        _output.Write(_buffer, 0, _buffer.Length);
    }
}

[HarmonyPatch, HarmonyWrapSafe]
file static class Patch
{
    [HarmonyPatch(typeof(BepInEx.Logging.Logger), nameof(BepInEx.Logging.Logger.InternalLogEvent))]
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

[Serializable]
file struct ErrorResult(string message)
{
    public string message = message;
}