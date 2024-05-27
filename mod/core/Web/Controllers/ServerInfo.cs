#nullable enable

using BepInEx.Bootstrap;

namespace ValheimWebLink.Web.Controllers;

[Controller]
public class ServerInfo : IController
{
    public string Route => "/serverinfo";
    public string HttpMethod => "GET";
    public string Description => "Returns vast server info";
    public List<QueryParamInfo> QueryParameters => []; //TODO: info with auth
    public bool RequiresAuth => false;

    public Task HandleRequest(HttpListenerRequest request, HttpListenerResponse response,
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
    public string? time;
    public int? day;
    public string? timeOfDay;
    public int playersCount;
    public string[] players;
    public string[] globalKeys;
    
    //TODO: only with auth
    public string[] adminList; 
    public string[] banList;
    
    public string[] mods;

    public static ServerInfoData Create()
    {
        var name = ZNet.m_ServerName;
        var version = Version.GetVersionString();
        var time = TimeUtils.GetCurrentTimeValue();
        double timeSeconds = ZNet.instance?.GetTimeSeconds() ?? 0;
        var day = EnvMan.instance?.GetDay(timeSeconds - (EnvMan.instance?.m_dayLengthSec ?? 0) * 0.15000000596046448)
                  ?? null;
        TimeOfDay? timeOfDay = null;
        if (EnvMan.IsDay()) timeOfDay = TimeOfDay.Day;
        else if (EnvMan.IsDaylight()) timeOfDay = TimeOfDay.Daylight;
        else if (EnvMan.IsAfternoon()) timeOfDay = TimeOfDay.Afternoon;
        else if (EnvMan.IsNight()) timeOfDay = TimeOfDay.Night;
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
            time = time.Item1 == -1 ? null : $"{time.Item1}:{time.Item2}",
            day = day,
            timeOfDay = timeOfDay.ToString(),
            playersCount = playersCount,
            players = players,
            globalKeys = globalKeys,
            adminList = adminList,
            banList = banList,
            mods = mods
        };
    }
}

file enum TimeOfDay
{
    Day,
    Daylight,
    Afternoon,
    Night
}