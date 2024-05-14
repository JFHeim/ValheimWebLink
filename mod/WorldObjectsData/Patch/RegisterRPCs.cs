using HarmonyLib;

namespace VWL_WorldObjectsData.Patch;

[HarmonyWrapSafe, HarmonyPatch]
file static class RegisterRPCs
{
    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
    [HarmonyPostfix]
    private static void AddRPCsPatch()
    {
        var zrp = ZRoutedRpc.instance;
        if (!zrp.m_functions.ContainsKey("VWL_SetPlayerData".GetStableHashCode()))
            zrp.Register<string, string>("VWL_SetPlayerData", HandlerSetPlayerData.SetPlayerData);

        if (!zrp.m_functions.ContainsKey("VWL_GetPlayerInventoryFromClient".GetStableHashCode()))
            zrp.Register<string>("VWL_GetPlayerInventoryFromClient", HandlerSendPlayerInventory.SendInventoryToServer);
    }
}