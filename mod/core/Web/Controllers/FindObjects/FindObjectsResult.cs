using static ValheimWebLink.RecordPrefabs;

namespace ValheimWebLink.Web.Controllers.FindObjects;

[Serializable]
public struct FindObjectsResult()
{
    public List<ObjectData> objects = [];

    public static FindObjectsResult Create(List<ZDO> list)
    {
        var result = new FindObjectsResult();
        // var hasWorldObjects = MobulesInstalled.Contains("WorldObjectsData");

        foreach (var zdo in list)
        {
            // if (hasWorldObjects)
            switch (GetObjectType(zdo.GetPrefab()))
            {
                case ObjectType.Other:
                    result.objects.Add(new ObjectData().Init(zdo));
                    break;
                case ObjectType.Player:
                    result.objects.Add(new PlayerData().Init(zdo));
                    break;
                case ObjectType.Creature:
                    result.objects.Add(new CreatureData().Init(zdo));
                    break;
                case ObjectType.Item:
                    result.objects.Add(new ItemData().Init(zdo));
                    break;
                case ObjectType.Structure:
                    result.objects.Add(new PieceData().Init(zdo));
                    break;
                case ObjectType.Container:
                    result.objects.Add(new ContainerData().Init(zdo));
                    break;
                case ObjectType.Portal:
                    result.objects.Add(new PortalData().Init(zdo));
                    break;
                case ObjectType.Door:
                    result.objects.Add(new DoorData().Init(zdo));
                    break;
                case ObjectType.Fireplace:
                    result.objects.Add(new FireplaceData().Init(zdo));
                    break;
            }
            // else result.objects.Add(new ObjectData().Init(zdo));
        }

        return result;
    }
}