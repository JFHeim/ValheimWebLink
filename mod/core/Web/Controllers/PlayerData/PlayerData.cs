#nullable enable
namespace ValheimWebLink.Web.Controllers.PlayerData;

[Serializable]
public struct PlayerData()
{
    public string name;
    public ulong id;
    public string platform;
    public bool? publicPosition;
    public bool isAdmin;
    public SimpleVector3 position;
    public bool alive;
    public int health;
    public int maxHealth;
    public int stamina;
    public int eitr;
    public bool inDebugFly;
    public bool isPVPEnabled;
    public string currentEmote;
    public bool inBed;
    public float noise;


    public float? randomSkillFactor;
    // public List<FoodData> food;
    // public InventoryData inventory;
    // public List<string> customData;

    public static async Task<(HttpStatusCode, string msg, PlayerData)> Create(string name)
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
        result.isPVPEnabled = plZdo.GetBool(ZDOVars.s_pvp);
        result.currentEmote = plZdo.GetString(ZDOVars.s_emote);
        result.inBed = plZdo.GetBool(ZDOVars.s_inBed);
        result.noise = plZdo.GetFloat(ZDOVars.s_noise);
        result.randomSkillFactor = plZdo.GetFloat(ZDOVars.s_randomSkillFactor);


        ZNet.PlayerInfo playerInfo = ZNet.instance.GetPlayerList().Find(x => x.m_name == name);
        var user = PrivilegeManager.ParseUser(playerInfo.m_host);
        result.id = user.id;
        result.platform = user.platform.ToString();
        result.publicPosition = playerInfo.m_publicPosition;
        result.position = playerInfo.m_position.ToSimpleVector3();
        result.isAdmin = ZNet.instance.PlayerIsAdmin(user.id.ToString());

        return (OK, string.Empty, result);
    }

    public async Task<(HttpStatusCode, string msg)> ApplyToPlayer(string name)
    {
        if (ZNet.instance == null || ZoneSystem.instance == null)
            return (ServiceUnavailable, "Game is not fully loaded");

        if (!name.IsGood()) return (BadRequest, "Invalid player name");

        return (NotImplemented, "Not implemented yet");

        return (OK, string.Empty);
    }
}