namespace ValheimWebLink.Web.Controllers.FindObjects;

[Serializable]
public class PlayerData : CreatureData
{
    public string playerName;
    public long playerID;
    public bool weaponLoaded;
    public bool debugFly;
    public float eitr;
    public float stamina;
    public bool pvp;
    public bool inBed;
    
    public PlayerData() => objectType = RecordPrefabs.ObjectType.Creature.ToString();

    public override ObjectData Init(ZDO zdo)
    {
        base.Init(zdo);
        playerName = zdo.GetString(ZDOVars.s_playerName, "...");
        playerID = zdo.GetLong(ZDOVars.s_playerID);
        weaponLoaded = zdo.GetBool(ZDOVars.s_weaponLoaded);
        debugFly = zdo.GetBool(ZDOVars.s_debugFly);
        eitr = zdo.GetFloat(ZDOVars.s_eitr);
        stamina = zdo.GetFloat(ZDOVars.s_stamina);
        pvp = zdo.GetBool(ZDOVars.s_pvp);
        inBed = zdo.GetBool(ZDOVars.s_inBed);

        return this;
    }
}