namespace ValheimWebLink.Web.Controllers.FindObjects;

[Serializable]
public class ContainerData : ObjectData
{
    public int itemsCount;
    public bool isInUse;
    public List<ItemInfo> items = [];

    public ContainerData() => objectType = RecordPrefabs.ObjectType.Container.ToString();

    public override ObjectData Init(ZDO zdo)
    {
        base.Init(zdo);
        isInUse = zdo.GetInt(ZDOVars.s_inUse) == 1;
        string base64String = zdo.GetString(ZDOVars.s_items);
        if (base64String.IsGood())
        {
            var pkg = new ZPackage(base64String);
            var inventory = new Inventory("_", null, 999, 999);
            inventory.Load(pkg);

            itemsCount = inventory.m_inventory.Count;
            foreach (var itemData in inventory.m_inventory)
                items.Add(new()
                {
                    prefabName = RecordPrefabs.GetItemPrefab(itemData.m_shared),
                    stack = itemData.m_stack,
                    durability = itemData.m_durability,
                    quality = itemData.m_quality,
                    variant = itemData.m_variant,
                    crafterName = itemData.m_crafterName,
                    gridPos = itemData.m_gridPos.ToVector2(),
                    equipped = itemData.m_equipped
                });
        }

        return this;
    }
}

public struct ItemInfo()
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