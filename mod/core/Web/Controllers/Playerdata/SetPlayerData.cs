namespace ValheimWebLink.Web.Controllers.Playerdata;

[Controller]
public class SetPlayerData : IController
{
    public string Route => "/playerdata/set";
    public string HttpMethod => "POST";

    public string Description =>
        "Provides way to override player data. Requires authentication. Requires WorldObjectsData module installed.";

    public bool RequiresAuth => true;

    public List<QueryParamInfo> QueryParameters =>
    [
        new("name", "string", "player name"),
        new("data", "json", "player data")
    ];

    public Task HandleRequest(HttpListenerRequest request, HttpListenerResponse response,
        Dictionary<string, string> queryParameters)
    {
        if (!queryParameters.TryGetValue("name", out var name))
        {
            WebApiManager.SendResponce(response, BadRequest, "Missing name parameter");
            return Task.CompletedTask;
        }

        if (!queryParameters.TryGetValue("data", out var data_string))
        {
            WebApiManager.SendResponce(response, BadRequest, "Missing data parameter");
            return Task.CompletedTask;
        }

        if (ZNet.instance == null || ZoneSystem.instance == null)
        {
            WebApiManager.SendResponce(response, ServiceUnavailable, "Game is not fully loaded");
            return Task.CompletedTask;
        }

        if (!name.IsGood())
        {
            WebApiManager.SendResponce(response, BadRequest, "Invalid player name");
            return Task.CompletedTask;
        }

        if (!ZNet.instance.GetPlayerList().Exists(x => x.m_name == name))
        {
            WebApiManager.SendResponce(response, BadRequest, "Player not found");
            return Task.CompletedTask;
        }

        var hasWorldObjects = MobulesInstalled.Contains("WorldObjectsData");
        if (!hasWorldObjects)
        {
            WebApiManager.SendResponce(response, NotImplemented, "Required module WorldObjectsData is not installed");
            return Task.CompletedTask;
        }

        Debug($"Sending player data to server: {data_string}");
        ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "VWL_SetPlayerData", name, data_string);

        WebApiManager.SendResponce(response, OK);
        return Task.CompletedTask;
    }
}