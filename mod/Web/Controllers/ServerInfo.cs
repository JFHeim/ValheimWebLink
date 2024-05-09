using BepInEx.Bootstrap;

namespace ValheimWebLink.Web.Controllers;

[Controller]
public class ServerInfo : IController
{
    public string Route => "/serverinfo";
    public string HttpMethod => "GET";
    public string Description => "Returns vast server info";
    public List<QueryParamInfo> QueryParameters => [];

    public Task HandleRequest(HttpListenerRequest request, HttpListenerResponse response, bool isAuthed,
        Dictionary<string, string> queryParameters)
    {
        WebApiManager.SendResponce(response, 200, "application/json", ServerInfoData.Create());
        return Task.CompletedTask;
    }
}

[Serializable]
file struct ServerInfoData
{
    public string name;
    public string version;
    public int worldSeed;
    public int playersCount;
    public string[] players;
    public string[] globalKeys;
    public string[] adminList;
    public string[] banList;
    public string[] mods;

    public static ServerInfoData Create()
    {
        var name = ZNet.m_ServerName;
        var version = Version.GetVersionString();
        var worldSeed = WorldGenerator.instance?.m_world?.m_seed ?? default;
        var playersCount = ZNet.instance?.GetNrOfPlayers() ?? 0;
        var players = ZNet.instance?.GetPlayerList().Select(x => x.m_name).ToArray() ?? [];
        var globalKeys = ZoneSystem.instance?.GetGlobalKeys().ToArray() ?? [];
        var adminList = ZNet.instance?.m_adminList.GetList().ToArray() ?? [];
        var banList = ZNet.instance?.m_bannedList.GetList().ToArray() ?? [];
        var mods = Chainloader.PluginInfos.Select(x => x.Key).ToArray();

        return new ServerInfoData
        {
            name = name,
            version = version,
            worldSeed = worldSeed,
            playersCount = playersCount,
            players = players,
            globalKeys = globalKeys,
            adminList = adminList,
            banList = banList,
            mods = mods
        };
    }
}