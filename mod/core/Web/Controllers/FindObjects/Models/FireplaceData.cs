namespace ValheimWebLink.Web.Controllers.FindObjects.Models;

[Serializable]
public class FireplaceData : PieceData
{
    public float fuel;

    public FireplaceData() => objectType = RecordPrefabs.ObjectType.Fireplace.ToString();

    public override async Task<ObjectData> Init(ZDO zdo)
    {
        await base.Init(zdo);
        fuel = zdo.GetFloat(ZDOVars.s_fuel);
        
        return this;
    }
}