using System.IO;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace ValheimWebLink.Web.Controllers;

[Controller]
public class GetLog : IController
{
    public string Route => "/getfulllog";
    public string HttpMethod => "GET";
    public string Description => "Returns logs from the log file";

    public void HandleRequest(HttpListenerRequest request, HttpListenerResponse response, bool isAuthed,
        Dictionary<string, string> queryParameters)
    {
        var path = Path.Combine(Paths.BepInExRootPath, "LogOutput.log");

        response.StatusCode = 200;
        response.ContentType = "text/plain";
        response.ContentEncoding = Encoding.UTF8;

        string responseString;
        try
        {
            using var fileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            using var streamReader = new StreamReader(fileStream);
            responseString = streamReader.ReadToEnd();
        }
        catch (IOException ex)
        {
            response.StatusCode = 500;
            responseString = $"Error reading file: {ex.GetType().Name} {ex.Message}";
        }

        byte[] buffer = Encoding.UTF8.GetBytes(responseString);
        response.ContentLength64 = buffer.Length;
        using Stream output = response.OutputStream;
        output.Write(buffer, 0, buffer.Length);
    }
}