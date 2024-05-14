namespace VWL_WorldObjectsData;

#nullable enable

[Serializable]
public struct InventoryData()
{
    public List<ItemInfo> items = [];
}

[Serializable]
public struct ItemInfo
{
    public string? prefabName;
    public int? stack;
    public float? durability;
    public int? quality;
    public int? variant;
    public string? crafterName;
    public SimpleVector2? gridPos;
}