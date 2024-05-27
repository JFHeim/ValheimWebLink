using ValheimWebLink.Web.Controllers.FindObjects.Models;

namespace ValheimWebLink.Web.Controllers.Playerdata;

[Controller]
public class GetPlayerData : IController
{
    public string Route => "/playerdata/get";
    public string HttpMethod => "GET";

    public string Description =>
        "Returns detail information about the player. Requires authentication. To get more data about the player, install WorldObjectsData module.";

    public List<QueryParamInfo> QueryParameters => [new("name", "string", "Name of the player")];

    public List<Permission> RequiredPermissions => [Permission.READ_objects];

    public async Task HandleRequest(HttpListenerRequest request, HttpListenerResponse response,
        Dictionary<string, string> queryParameters, List<Permission> userPermissions)
    {
        if (!queryParameters.TryGetValue("name", out var name))
        {
            WebApiManager.SendResponce(response, BadRequest, "Missing name parameter");
            return;
        }

        (HttpStatusCode code, string msg, PlayerData playerData) pD = await CreatePlayerData(name);

        if (pD.code == OK)
        {
            WebApiManager.SendResponce(response, pD.code, "application/json", pD.playerData);
            return;
        }

        WebApiManager.SendResponce(response, pD.code, pD.msg);
    }

    private static async Task<(HttpStatusCode, string msg, PlayerData)> CreatePlayerData(string name)
    {
        var result = new PlayerData();

        if (ZNet.instance == null || ZoneSystem.instance == null)
            return (ServiceUnavailable, "Game is not fully loaded", result);

        if (!name.IsGood()) return (BadRequest, "Invalid player name", result);

        var playerZdo =
            await ZoneSystem.instance.GetWorldObjectsAsync(x => x.GetPrefab() == "Player".GetStableHashCode() &&
                                                                x.GetString(ZDOVars.s_playerName) == name);

        if (playerZdo.Count == 0) return (NotFound, "Player not found", result);
        if (playerZdo.Count > 1) return (InternalServerError, "Too many players found with same name", result);
        var plZdo = playerZdo.Single();
        await result.Init(plZdo);

        return (OK, string.Empty, result);
    }
}