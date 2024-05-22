using HarmonyLib;

namespace VWL_PlayerEye.Patch;

[HarmonyWrapSafe, HarmonyPatch]
[ClientSidePatch, ServerSidePatch]
file static class RegisterRPCs
{
    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
    [HarmonyPostfix]
    private static void AddRPCsPatch()
    {
        var zrp = ZRoutedRpc.instance;
        if (!zrp.m_functions.ContainsKey("VWL_RequestEye".GetStableHashCode()))
            zrp.Register<string, string>("VWL_RequestEye", RequestEye);
        if (!zrp.m_functions.ContainsKey("VWL_StopEye".GetStableHashCode()))
            zrp.Register<string>("VWL_StopEye", StopEye);
    }

    private static void StopEye(long _, string plName)
    {
        var pl = m_localPlayer;
        if (!pl || pl.GetPlayerName() != plName) return;
        StopEye();
    }

    private static void StopEye() => ClientLogic.StopEye();

    private static void RequestEye(long _, string plName, string serverUrl)
    {
        var pl = m_localPlayer;
        if (!pl || pl.GetPlayerName() != plName) return;
        RequestEye(plName, serverUrl);
    }

    private static void RequestEye(string plName, string serverUrl)
    {
        ClientLogic.ConnectToServer(plName, serverUrl);
    }
}