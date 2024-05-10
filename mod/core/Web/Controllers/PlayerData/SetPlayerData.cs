namespace ValheimWebLink.Web.Controllers.PlayerData;

[Controller]
public class SetPlayerData : IController
{
    public string Route => "/playerdata/set";
    public string HttpMethod => "POST";
    public string Description => "Provides way to override player data. Requires authentication. Requires PlayerData module installed.";

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

        var playerData = JSON.ToObject<PlayerData>(data_string);
        var (status, msg) = await playerData.ApplyToPlayer(name);
        WebApiManager.SendResponce(response, status, msg);
    }
}