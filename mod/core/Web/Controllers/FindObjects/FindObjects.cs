namespace ValheimWebLink.Web.Controllers.FindObjects;

[Controller]
public class FindObjects : IController
{
    public string Route => "/findobjects";
    public string HttpMethod => "GET";

    public string Description =>
        "Searches for objects in the world in given range. To see full data of objects, install WorldObjectsData module.";

    public List<QueryParamInfo> QueryParameters =>
    [
        new("centerpoint", "Vector2", "Center point of the search"),
        new("radius", "float", "Radius of the search")
    ];

    public bool RequiresAuth => true;

    public async Task HandleRequest(HttpListenerRequest request, HttpListenerResponse response,
        Dictionary<string, string> queryParameters)
    {
        if (ZNet.instance == null || ZoneSystem.instance == null)
            WebApiManager.SendResponce(response, ServiceUnavailable, "Game is not fully loaded");

        if (!queryParameters.TryGetValue("centerpoint", out var centerPoint_string) || !SimpleVector2.TryParse
                (centerPoint_string, out var center))
        {
            WebApiManager.SendResponce(response, BadRequest, "Parameter centerpoint is missing or invalid");
            return;
        }

        if (!queryParameters.TryGetValue("radius", out var radius_string)
            || !float.TryParse(radius_string, out var radius))
        {
            WebApiManager.SendResponce(response, BadRequest, "Parameter radius is missing or invalid");
            return;
        }

        var objects = await ZoneSystem.instance.GetWorldObjectsAsync(x =>
            Vector2.Distance(center, x.GetPosition().ToV2()) <= radius);

        var result = await FindObjectsResult.Create(objects);
        WebApiManager.SendResponce(response, OK, "application/json", result);
    }
}