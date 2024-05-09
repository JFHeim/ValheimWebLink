using HarmonyLib;

namespace ValheimWebLink.Web.Controllers.FindObjects;

[HarmonyPatch, HarmonyWrapSafe]
[HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
public static class RecordPrefabNames
{
    public static readonly Dictionary<int, string> prefabNames = new();

    [UsedImplicitly]
    private static void Postfix(ZNetScene __instance)
    {
        foreach (var prefab in __instance.m_prefabs)
        {
            var hashCode = prefab.name.GetStableHashCode();
            if (prefabNames.ContainsKey(hashCode)) continue;
            prefabNames.Add(hashCode, prefab.name);
        }
    }
}