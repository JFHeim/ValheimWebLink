namespace ValheimWebLink.Web.Controllers.FindObjects;

[Serializable]
public class DoorData : PieceData
{
    public bool isOpen;

    public DoorData() => objectType = RecordPrefabs.ObjectType.Door.ToString();

    public override ObjectData Init(ZDO zdo)
    {
        base.Init(zdo);
        isOpen = zdo.GetInt(ZDOVars.s_state) == 0;

        return this;
    }
}