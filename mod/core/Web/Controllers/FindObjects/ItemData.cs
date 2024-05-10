namespace ValheimWebLink.Web.Controllers.FindObjects;

[Serializable]
public class ItemData : ObjectData
{
    public long spawnTime;
    public float durability;
    public int stack;
    public int quality;
    public int variant;
    public long crafterID;
    public string crafterName;
    public int dataCount;

    public ItemData() => objectType = RecordPrefabs.ObjectType.Item.ToString();

    public override ObjectData Init(ZDO zdo)
    {
        base.Init(zdo);
        spawnTime = zdo.GetLong(ZDOVars.s_spawnTime);
        durability = zdo.GetFloat(ZDOVars.s_durability);
        stack = zdo.GetInt(ZDOVars.s_stack);
        quality = zdo.GetInt(ZDOVars.s_quality);
        variant = zdo.GetInt(ZDOVars.s_variant);
        crafterID = zdo.GetLong(ZDOVars.s_crafterID);
        crafterName = zdo.GetString(ZDOVars.s_crafterName);
        dataCount = zdo.GetInt(ZDOVars.s_dataCount);
        //TODO: Load custom data

        return this;
    }
}