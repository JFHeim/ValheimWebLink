namespace ValheimWebLink.Web.Controllers;

public interface IController
{
    public string Route { get; }
    public string HttpMethod { get; }
    public string Description { get; }
    public List<QueryParamInfo> QueryParameters { get; }
    public List<Permission> RequiredPermissions { get; }

    public Task HandleRequest(HttpListenerRequest request, HttpListenerResponse response,
        Dictionary<string, string> queryParameters, List<Permission> userPermissions);
}