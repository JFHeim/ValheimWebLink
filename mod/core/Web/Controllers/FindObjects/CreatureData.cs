namespace ValheimWebLink.Web.Controllers.FindObjects;

[Serializable]
public class CreatureData : ObjectData
{
    public bool tamed;
    public int level;
    public float health;
    public float maxHealth;
    public float noise;

    public CreatureData() => objectType = RecordPrefabs.ObjectType.Creature.ToString();

    public override ObjectData Init(ZDO zdo)
    {
        base.Init(zdo);
        tamed = zdo.GetBool(ZDOVars.s_tamed);
        level = zdo.GetInt(ZDOVars.s_level);
        health = zdo.GetFloat(ZDOVars.s_health);
        maxHealth = zdo.GetFloat(ZDOVars.s_maxHealth);
        noise = zdo.GetFloat(ZDOVars.s_noise);

        return this;
    }
}