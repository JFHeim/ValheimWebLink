namespace ValheimWebLink.Web.Controllers.FindObjects.Models;

[Serializable]
public class DoorData : PieceData
{
    public bool isOpen;

    public DoorData() => objectType = RecordPrefabs.ObjectType.Door.ToString();

    public override async Task<ObjectData> Init(ZDO zdo)
    {
        base.Init(zdo);
        isOpen = zdo.GetInt(ZDOVars.s_state) == 0;

        return this;
    }
}