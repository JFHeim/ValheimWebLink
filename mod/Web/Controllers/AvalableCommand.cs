using System.IO;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace ValheimWebLink.Web.Controllers;

[Controller]
public class AvalibleCommand : IController
{
    public string Route => "/avaliblecommands";
    public string HttpMethod => "GET";
    public string Description => "Returns all known ingame console commands";

    public void HandleRequest(HttpListenerRequest request, HttpListenerResponse response, bool isAuthed,
        Dictionary<string, string> queryParameters)
    {
        response.StatusCode = 200;
        response.ContentType = "application/json";
        response.ContentEncoding = Encoding.UTF8;
        string responseString = JSON.ToJSON(Result.Create());
        byte[] buffer = Encoding.UTF8.GetBytes(responseString);
        response.ContentLength64 = buffer.Length;
        using Stream output = response.OutputStream;
        output.Write(buffer, 0, buffer.Length);
    }
}

[Serializable]
file struct CommandInfo()
{
    public string command;
    public string description;
    public List<string> tabOptions;
}

[Serializable]
file struct Result()
{
    public List<CommandInfo> commands = [];

    public static Result Create()
    {
        var ret = new Result();
        foreach (var cm in Terminal.commands.Values)
            ret.commands.Add(new()
            {
                command = cm.Command,
                description = cm.Description,
                tabOptions = cm.m_tabOptions
            });

        return ret;
    }
}