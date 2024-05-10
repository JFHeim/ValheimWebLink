using HarmonyLib;

namespace ValheimWebLink;

[HarmonyPatch, HarmonyWrapSafe]
[HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
public static class RecordPrefabs
{
    public static readonly Dictionary<int, string> prefabNames = new();
    public static readonly Dictionary<ItemDrop.ItemData.SharedData, string> itemPrefabNames = new();
    public static readonly Dictionary<int, ObjectType> objectTypes = new();

    [UsedImplicitly]
    private static void Postfix(ZNetScene __instance)
    {
        foreach (var prefab in __instance.m_prefabs)
        {
            var hashCode = prefab.name.GetStableHashCode();
            if (!prefabNames.ContainsKey(hashCode))
                prefabNames.Add(hashCode, prefab.name);

            if (!objectTypes.ContainsKey(hashCode))
                if (prefab.GetComponent<TeleportWorld>())
                    objectTypes.Add(hashCode, ObjectType.Portal);
                else if (prefab.GetComponent<Character>())
                    objectTypes.Add(hashCode, ObjectType.Creature);
                else if (prefab.GetComponent<ItemDrop>())
                    objectTypes.Add(hashCode, ObjectType.Item);
                else if (prefab.GetComponent<Container>())
                    objectTypes.Add(hashCode, ObjectType.Container);
                else if (prefab.GetComponent<Door>())
                    objectTypes.Add(hashCode, ObjectType.Door);
                else if (prefab.GetComponent<Fireplace>())
                    objectTypes.Add(hashCode, ObjectType.Fireplace);
                else if (prefab.GetComponent<Piece>())
                    objectTypes.Add(hashCode, ObjectType.Structure);
                else objectTypes.Add(hashCode, ObjectType.Other);
        }
    }

    public static string GetItemPrefab(ItemDrop.ItemData.SharedData sharedData) =>
        itemPrefabNames.TryGetValue(sharedData, out var name) ? name : "Unknown";

    public static ObjectType GetObjectType(int hashCode) =>
        objectTypes.TryGetValue(hashCode, out var result) ? result : ObjectType.Other;

    public enum ObjectType
    {
        Other,
        Player,
        Creature,
        Item,
        Structure,
        Container,
        Portal,
        Door,
        Fireplace
    }
}