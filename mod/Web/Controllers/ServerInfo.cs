using System.IO;
using BepInEx.Bootstrap;
using ValheimWebLink.Models;

namespace ValheimWebLink.Web.Controllers;

[Controller]
public class ServerInfo : IController
{
    public string Route => "/serverinfo";
    public string HttpMethod => "GET";
    public string Description => "Returns vast server info";

    public void HandleRequest(HttpListenerRequest request, HttpListenerResponse response, bool isAuthed,
        Dictionary<string, string> queryParameters)
    {
        response.StatusCode = 200;
        response.ContentType = "application/json";
        response.ContentEncoding = Encoding.UTF8;
        var data = ServerInfoData.Create();
        string responseString = JSON.ToJSON(data);
        byte[] buffer = Encoding.UTF8.GetBytes(responseString);
        response.ContentLength64 = buffer.Length;
        using Stream output = response.OutputStream;
        output.Write(buffer, 0, buffer.Length);
    }
}

[Serializable]
file struct ServerInfoData
{
    public string name;
    public string version;
    public int worldSeed;
    public int playersCount;
    public PlayerInfo[] players;
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
        var players = ZNet.instance?.GetPlayerList().Select(PlayerInfo.Create).ToArray() ?? [];
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