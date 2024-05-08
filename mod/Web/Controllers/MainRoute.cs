using System.IO;

namespace ValheimWebLink.Web.Controllers;

[Controller]
public class MainRoute : IController
{
    public string Route => "/";
    public string HttpMethod => "GET";

    public string Description => "Returns all controllers info";

    public void HandleRequest(HttpListenerRequest request, HttpListenerResponse response, bool isAuthed,
        Dictionary<string, string> queryParameters)
    {
        response.StatusCode = 200;
        response.ContentType = "application/json";
        response.ContentEncoding = Encoding.UTF8;
        string responseString = JSON.ToJSON(new AllControllersInfo());
        byte[] buffer = Encoding.UTF8.GetBytes(responseString);
        response.ContentLength64 = buffer.Length;
        using Stream output = response.OutputStream;
        output.Write(buffer, 0, buffer.Length);
    }
}

[Serializable]
file struct AllControllersInfo()
{
    public ControllerInfo[] controllers = WebApiManager.Controllers.Select(ControllerInfo.FromIController).ToArray();
}

[Serializable]
file struct ControllerInfo
{
    public string route;
    public string httpMethod;
    public string description;

    public static ControllerInfo FromIController(IController controller) =>
        new()
        {
            route = controller.Route,
            httpMethod = controller.HttpMethod,
            description = controller.Description
        };
}