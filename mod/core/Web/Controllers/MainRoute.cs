using UnityEngine.Serialization;

namespace ValheimWebLink.Web.Controllers;

[Controller]
public class MainRoute : IController
{
    public string Route => "/";
    public string HttpMethod => "GET";
    public string Description => "Returns all routes info";
    public List<QueryParamInfo> QueryParameters => [];
    public bool RequiresAuth => false;

    internal static List<RouteInfoJSON> aditionalRoutes = [];

    public Task HandleRequest(HttpListenerRequest request, HttpListenerResponse response,
        Dictionary<string, string> queryParameters)
    {
        WebApiManager.SendResponce(response, OK, "application/json", new AllControllersInfo());
        return Task.CompletedTask;
    }

    public static void AddRouterInfo(RouteInfoJSON info) => aditionalRoutes.Add(info);
}

[Serializable]
file struct AllControllersInfo
{
    public RouteInfoJSON[] routes;

    public AllControllersInfo()
    {
        var routesList = WebApiManager.Controllers.Select(FromIController).ToList();
        foreach (var info in MainRoute.aditionalRoutes) routesList.Add(info);
        routes = routesList.ToArray();
    }

    private static RouteInfoJSON FromIController(IController controller) =>
        new()
        {
            protocol = "http",
            route = controller.Route,
            httpMethod = controller.HttpMethod,
            description = controller.Description,
            queryParameters = controller.QueryParameters,
            RequiresAuth = controller.RequiresAuth
        };
}