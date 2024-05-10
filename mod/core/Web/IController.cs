namespace ValheimWebLink.Web.Controllers;

public interface IController
{
    public string Route { get; }
    public string HttpMethod { get; }
    public string Description { get; }
    public List<QueryParamInfo> QueryParameters { get; }

    public Task HandleRequest(HttpListenerRequest request, HttpListenerResponse response, bool isAuthed,
        Dictionary<string, string> queryParameters);
}