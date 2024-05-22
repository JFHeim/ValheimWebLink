namespace ValheimWebLink.Web.Controllers.Map;

[Controller]
public class MapRequest : IController
{
    public string Route => "/mapdata";
    public string HttpMethod => "GET";
    public string Description => "";
    public List<QueryParamInfo> QueryParameters => [];
    public bool RequiresAuth => true;

    public Task HandleRequest(HttpListenerRequest request, HttpListenerResponse response,
        Dictionary<string, string> queryParameters)
    {
        
        throw new NotImplementedException();
    }
}