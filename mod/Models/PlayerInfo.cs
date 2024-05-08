namespace ValheimWebLink.Models;

public struct PlayerInfo
{
    public string nickname;
    public ulong id;
    public string platform;
    public bool publicPosition;
    public SimpleVector3 position;
    public bool isAdmin;

    public static PlayerInfo Create(ZNet.PlayerInfo arg)
    {
        var user = PrivilegeManager.ParseUser(arg.m_host);
        return new()
        {
            nickname = arg.m_name,
            id = user.id,
            platform = user.platform.ToString(),
            publicPosition = arg.m_publicPosition,
            position = arg.m_position.ToSimpleVector3(),
            isAdmin = ZNet.instance.PlayerIsAdmin(user.id.ToString())
        };
    }
}