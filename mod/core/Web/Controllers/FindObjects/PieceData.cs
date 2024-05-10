namespace ValheimWebLink.Web.Controllers.FindObjects;

[Serializable]
public class PieceData : ObjectData
{
    public string m_creator;
    public float health;
    public float support;
    
    public PieceData() => objectType = RecordPrefabs.ObjectType.Structure.ToString();

    public override ObjectData Init(ZDO zdo)
    {
        base.Init(zdo);
        m_creator = zdo.GetString(ZDOVars.s_creatorName, "");
        if (!m_creator.IsGood()) m_creator = zdo.GetLong(ZDOVars.s_creator).ToString();
        health = zdo.GetFloat(ZDOVars.s_health);
        support = zdo.GetFloat(ZDOVars.s_support);

        return this;
    }
}