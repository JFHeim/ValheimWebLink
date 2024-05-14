using ValheimWebLink.Web.Controllers.FindObjects.Models;
using static ValheimWebLink.RecordPrefabs;

namespace ValheimWebLink.Web.Controllers.FindObjects;

[Serializable]
public struct FindObjectsResult()
{
    public List<ObjectData> objects = [];

    public static async Task<FindObjectsResult> Create(List<ZDO> list)
    {
        var result = new FindObjectsResult();
        // var hasWorldObjects = MobulesInstalled.Contains("WorldObjectsData");

        foreach (var zdo in list)
        {
            // if (hasWorldObjects)
            switch (GetObjectType(zdo.GetPrefab()))
            {
                case ObjectType.Other:
                    result.objects.Add(await new ObjectData().Init(zdo));
                    break;
                case ObjectType.Player:
                    result.objects.Add(await new PlayerData().Init(zdo, true));
                    break;
                case ObjectType.Creature:
                    result.objects.Add(await new CreatureData().Init(zdo));
                    break;
                case ObjectType.Item:
                    result.objects.Add(await new ItemData().Init(zdo));
                    break;
                case ObjectType.Structure:
                    result.objects.Add(await new PieceData().Init(zdo));
                    break;
                case ObjectType.Container:
                    result.objects.Add(await new ContainerData().Init(zdo));
                    break;
                case ObjectType.Portal:
                    result.objects.Add(await new PortalData().Init(zdo));
                    break;
                case ObjectType.Door:
                    result.objects.Add(await new DoorData().Init(zdo));
                    break;
                case ObjectType.Fireplace:
                    result.objects.Add(await new FireplaceData().Init(zdo));
                    break;
            }
            // else result.objects.Add(new ObjectData().Init(zdo));
        }

        return result;
    }
}