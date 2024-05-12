namespace ValheimWebLink.Web.Controllers.FindObjects.Models;

[Serializable]
public class PortalData : PieceData
{
    public string tag;
    public string tagauthor;

    public PortalData() => objectType = RecordPrefabs.ObjectType.Portal.ToString();

    public override async Task<ObjectData> Init(ZDO zdo)
    {
        base.Init(zdo);
        tag = zdo.GetString(ZDOVars.s_tag);
        tagauthor = zdo.GetString(ZDOVars.s_tagauthor);

        return this;
    }
}