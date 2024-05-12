using ValheimWebLink.Web.Controllers.FindObjects.Models;

namespace ValheimWebLink.Web.Controllers.Playerdata;

[Controller]
public class SetPlayerData : IController
{
    public string Route => "/playerdata/set";
    public string HttpMethod => "POST";

    public string Description =>
        "Provides way to override player data. Requires authentication. Requires WorldObjectsData module installed.";

    public List<QueryParamInfo> QueryParameters =>
    [
        new("name", "string", "player name"),
        new("data", "json", "player data")
    ];

    public async Task HandleRequest(HttpListenerRequest request, HttpListenerResponse response, bool isAuthed,
        Dictionary<string, string> queryParameters)
    {
        if (!isAuthed)
        {
            WebApiManager.SendResponce(response, Unauthorized);
            return;
        }

        if (!queryParameters.TryGetValue("name", out var name))
        {
            WebApiManager.SendResponce(response, BadRequest, "Missing name parameter");
            return;
        }

        if (!queryParameters.TryGetValue("data", out var data_string))
        {
            WebApiManager.SendResponce(response, BadRequest, "Missing data parameter");
            return;
        }

        if (ZNet.instance == null || ZoneSystem.instance == null)
        {
            WebApiManager.SendResponce(response, ServiceUnavailable, "Game is not fully loaded");
            return;
        }

        if (!name.IsGood())
        {
            WebApiManager.SendResponce(response, BadRequest, "Invalid player name");
            return;
        }

        if (!ZNet.instance.GetPlayerList().Exists(x => x.m_name == name))
        {
            WebApiManager.SendResponce(response, BadRequest, "Player not found");
            return;
        }

        var hasWorldObjects = MobulesInstalled.Contains("WorldObjectsData");
        if (!hasWorldObjects)
        {
            WebApiManager.SendResponce(response, NotImplemented, "Required module WorldObjectsData is not installed");
            return;
        }

        Debug($"Sending player data to server: {data_string}");
        ZRoutedRpc.instance.InvokeRoutedRPC(ZRoutedRpc.Everybody, "VWL_SetPlayerData", name, data_string);

        WebApiManager.SendResponce(response, OK);
    }
}