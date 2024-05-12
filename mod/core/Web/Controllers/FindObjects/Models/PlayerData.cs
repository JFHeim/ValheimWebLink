using HarmonyLib;

namespace ValheimWebLink.Web.Controllers.FindObjects.Models;

[Serializable]
public class PlayerData : CreatureData
{
    public string name;
    public long playerID;
    public ulong platformId;
    public string platform;
    public bool weaponLoaded;
    public bool publicPosition;
    public bool isAdmin;
    public bool inDebugFly;
    public float eitr;
    public float stamina;
    public bool pvp;
    public bool inBed;
    public bool alive;
    public InventoryData inventory;

    public PlayerData() => objectType = RecordPrefabs.ObjectType.Creature.ToString();

    public override async Task<ObjectData> Init(ZDO zdo)
    {
        base.Init(zdo);
        name = zdo.GetString(ZDOVars.s_playerName, "...");
        playerID = zdo.GetLong(ZDOVars.s_playerID);
        weaponLoaded = zdo.GetBool(ZDOVars.s_weaponLoaded);
        inDebugFly = zdo.GetBool(ZDOVars.s_debugFly);
        eitr = zdo.GetFloat(ZDOVars.s_eitr);
        stamina = zdo.GetFloat(ZDOVars.s_stamina);
        pvp = zdo.GetBool(ZDOVars.s_pvp);
        inBed = zdo.GetBool(ZDOVars.s_inBed);

        var hasWorldObjects = MobulesInstalled.Contains("WorldObjectsData");
        if (hasWorldObjects)
        {
            var inventoryJson = await AskClientForInventory();
            Debug($"Got inventory: {inventoryJson}");
        }

        return this;
    }

    private async Task<string> AskClientForInventory()
    {
        ZRoutedRpc.instance.InvokeRoutedRPC("VWL_GetPlayerInventoryFromClient", name);

        while (true)
        {
            await Task.Delay(100);
            if (!RegisterRPCs.dataJson.TryGetValue(name, out var data)) continue;
            RegisterRPCs.dataJson.Remove(name);
            return data;
        }
    }
}

[HarmonyWrapSafe, HarmonyPatch]
file static class RegisterRPCs
{
    public static Dictionary<string, string> dataJson;

    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
    [HarmonyPostfix]
    private static void AddRPCsPatch()
    {
        var hasWorldObjects = MobulesInstalled.Contains("WorldObjectsData");

        if (!hasWorldObjects) return;

        var zrp = ZRoutedRpc.instance;
        if (!zrp.m_functions.ContainsKey("VWL_GotInventory_Server".GetStableHashCode()))
            zrp.Register<string, string>("VWL_GotInventory_Server", OnGetInventory);
    }

    private static void OnGetInventory(long _, string name, string dataJson)
    {
        RegisterRPCs.dataJson.Add(name, dataJson);
    }
}