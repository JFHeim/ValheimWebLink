using BepInEx;

namespace ValheimWebLink.Web.Controllers;

[Controller]
public class GetLog : IController
{
    public string Route => "/getfulllog";
    public string HttpMethod => "GET";
    public string Description => "Returns logs from the log file";
    public List<QueryParamInfo> QueryParameters => [];
    public bool RequiresAuth => true;

    public Task HandleRequest(HttpListenerRequest request, HttpListenerResponse response,
        Dictionary<string, string> queryParameters)
    {
        string responseString;
        try
        {
            using var fileStream = File.Open(Path.Combine(Paths.BepInExRootPath, "LogOutput.log"), FileMode.Open,
                FileAccess.Read, FileShare.ReadWrite);
            using var streamReader = new StreamReader(fileStream);
            responseString = streamReader.ReadToEnd();
        }
        catch (IOException ex)
        {
            responseString = $"Error reading file: {ex.GetType().Name} {ex.Message}";
        }

        WebApiManager.SendResponce(response,
            responseString.StartsWith("Error") ? InternalServerError : OK, responseString);

        return Task.CompletedTask;
    }
}