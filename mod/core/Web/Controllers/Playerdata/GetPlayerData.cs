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
        result.name = name;
        result.position = plZdo.GetPosition();
        result.health = (int)plZdo.GetFloat(ZDOVars.s_health);
        result.maxHealth = (int)plZdo.GetFloat(ZDOVars.s_maxHealth);
        result.stamina = (int)plZdo.GetFloat(ZDOVars.s_stamina);
        result.eitr = (int)plZdo.GetFloat(ZDOVars.s_eitr);
        result.alive = !plZdo.GetBool(ZDOVars.s_dead);
        result.inDebugFly = plZdo.GetBool(ZDOVars.s_debugFly);
        result.pvp = plZdo.GetBool(ZDOVars.s_pvp);
        // result.currentEmote = plZdo.GetString(ZDOVars.s_emote);
        result.inBed = plZdo.GetBool(ZDOVars.s_inBed);
        result.noise = plZdo.GetFloat(ZDOVars.s_noise);
        result.playerID = plZdo.GetLong(ZDOVars.s_playerID);
        // result.randomSkillFactor = plZdo.GetFloat(ZDOVars.s_randomSkillFactor);


        ZNet.PlayerInfo playerInfo = ZNet.instance.GetPlayerList().Find(x => x.m_name == name);
        var user = PrivilegeManager.ParseUser(playerInfo.m_host);
        result.platformId = user.id;
        result.platform = user.platform.ToString();
        result.publicPosition = playerInfo.m_publicPosition;
        result.position = playerInfo.m_position.ToSimpleVector3();
        result.isAdmin = ZNet.instance.PlayerIsAdmin(user.id.ToString());

        return (OK, string.Empty, result);
    }
}