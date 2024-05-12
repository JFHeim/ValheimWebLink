namespace ValheimWebLink.Web.Controllers.FindObjects.Models;

[Serializable]
public class ContainerData : ObjectData
{
    public int itemsCount;
    public bool isInUse;
    public InventoryData inventoryData;

    public ContainerData() => objectType = RecordPrefabs.ObjectType.Container.ToString();

    public override async Task<ObjectData> Init(ZDO zdo)
    {
        base.Init(zdo);
        isInUse = zdo.GetInt(ZDOVars.s_inUse) == 1;
        string base64String = zdo.GetString(ZDOVars.s_items);
        if (base64String.IsGood())
        {
            var pkg = new ZPackage(base64String);
            Debug($"Creating inventory for {prefabName}");
            inventoryData = new()
            {
                items = LoadItems(pkg)
            };
            Debug($"Items in {prefabName} added");
            itemsCount = inventoryData.items.Count;
        }

        return this;
    }

    public List<ItemInfo> LoadItems(ZPackage pkg)
    {
        List<ItemInfo> result = [];
        int num1 = pkg.ReadInt();
        int num2 = pkg.ReadInt();
        if (num1 == 106)
        {
            for (int index1 = 0; index1 < num2; ++index1)
            {
                string name = pkg.ReadString();
                int stack = pkg.ReadInt();
                float durability = pkg.ReadSingle();
                Vector2i pos = pkg.ReadVector2i();
                bool equipped = pkg.ReadBool();
                int quality = pkg.ReadInt();
                int variant = pkg.ReadInt();
                long crafterID = pkg.ReadLong();
                string crafterName = pkg.ReadString();
                Dictionary<string, string> customData = new Dictionary<string, string>();
                int num3 = pkg.ReadInt();
                for (int index2 = 0; index2 < num3; ++index2)
                    customData[pkg.ReadString()] = pkg.ReadString();
                int worldLevel = pkg.ReadInt();
                bool pickedUp = pkg.ReadBool();
                if (name != "")
                {
                    result.Add(new()
                    {
                        prefabName = name,
                        stack = stack,
                        durability = durability,
                        quality = quality,
                        variant = variant,
                        crafterName = crafterName,
                        gridPos = pos.ToVector2(),
                        equipped = equipped
                    });
                }
            }
        } else
        {
            for (int index3 = 0; index3 < num2; ++index3)
            {
                string name = pkg.ReadString();
                int stack = pkg.ReadInt();
                float durability = pkg.ReadSingle();
                Vector2i pos = pkg.ReadVector2i();
                bool equipped = pkg.ReadBool();
                int quality = 1;
                if (num1 >= 101)
                    quality = pkg.ReadInt();
                int variant = 0;
                if (num1 >= 102)
                    variant = pkg.ReadInt();
                long crafterID = 0;
                string crafterName = "";
                if (num1 >= 103)
                {
                    crafterID = pkg.ReadLong();
                    crafterName = pkg.ReadString();
                }

                Dictionary<string, string> customData = new Dictionary<string, string>();
                if (num1 >= 104)
                {
                    int num4 = pkg.ReadInt();
                    for (int index4 = 0; index4 < num4; ++index4)
                    {
                        string key = pkg.ReadString();
                        string str = pkg.ReadString();
                        customData[key] = str;
                    }
                }

                int worldLevel = 0;
                if (num1 >= 105)
                    worldLevel = pkg.ReadInt();
                bool pickedUp = false;
                if (num1 >= 106)
                    pickedUp = pkg.ReadBool();
                if (name != "")
                {
                    result.Add(new()
                    {
                        prefabName = name,
                        stack = stack,
                        durability = durability,
                        quality = quality,
                        variant = variant,
                        crafterName = crafterName,
                        gridPos = pos.ToVector2(),
                        equipped = equipped
                    });
                }
            }
        }

        return result;
    }
}

[Serializable]
public struct InventoryData()
{
    public List<ItemInfo> items = [];
}

public struct ItemInfo
{
    public string prefabName;
    public int stack;
    public float durability;
    public int quality;
    public int variant;
    public string crafterName;
    public SimpleVector2 gridPos;
    public bool equipped;
}