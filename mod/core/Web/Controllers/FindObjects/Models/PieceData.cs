namespace ValheimWebLink.Web.Controllers.FindObjects.Models;

[Serializable]
public class PieceData : ObjectData
{
    public string m_creator;
    public float health;
    public float support;

    public PieceData() => objectType = RecordPrefabs.ObjectType.Structure.ToString();

    public override async Task<ObjectData> Init(ZDO zdo)
    {
        await base.Init(zdo);
        m_creator = zdo.GetString(ZDOVars.s_creatorName);
        if (!m_creator.IsGood()) m_creator = zdo.GetLong(ZDOVars.s_creator).ToString();
        health = zdo.GetFloat(ZDOVars.s_health);
        support = zdo.GetFloat(ZDOVars.s_support);

        return this;
    }
}