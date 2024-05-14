namespace ValheimWebLink.Web.Controllers;

[Controller]
public class MainRoute : IController
{
    public string Route => "/";
    public string HttpMethod => "GET";
    public string Description => "Returns all controllers info";
    public List<QueryParamInfo> QueryParameters => [];

    public Task HandleRequest(HttpListenerRequest request, HttpListenerResponse response, bool isAuthed,
        Dictionary<string, string> queryParameters)
    {
        WebApiManager.SendResponce(response, OK, "application/json", new AllControllersInfo());
        return Task.CompletedTask;
    }
}

[Serializable]
file struct AllControllersInfo()
{
    public ControllerInfo[] controllers = WebApiManager.Controllers.Select(ControllerInfo.FromIController).ToArray();
}

[Serializable]
file struct ControllerInfo()
{
    public string route;
    public string httpMethod;
    public string description;
    public List<QueryParamInfo> queryParameters = [];

    public static ControllerInfo FromIController(IController controller) =>
        new()
        {
            route = controller.Route,
            httpMethod = controller.HttpMethod,
            description = controller.Description,
            queryParameters = controller.QueryParameters
        };
}