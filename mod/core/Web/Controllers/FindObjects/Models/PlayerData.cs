using HarmonyLib;

#nullable enable

namespace ValheimWebLink.Web.Controllers.FindObjects.Models;

[Serializable]
public class PlayerData : CreatureData
{
    public string? name;
    public long? playerID;
    public ulong? platformId;
    public string? platform;
    public bool? weaponLoaded;
    public bool? publicPosition;
    public bool? isAdmin;
    public bool? inDebugFly;
    public float? eitr;
    public float? stamina;
    public bool? pvp;
    public bool? inBed;
    public bool? alive;
    public InventoryData? inventory;

    public PlayerData() => objectType = RecordPrefabs.ObjectType.Creature.ToString();

    public override async Task<ObjectData> Init(ZDO zdo) => await Init(zdo, false);

    public async Task<PlayerData> Init(ZDO zdo, bool onlyName)
    {
        await base.Init(zdo);
        tamed = null;
        level = null;

        name = zdo.GetString(ZDOVars.s_playerName, "...");
        if (onlyName) return this;
        playerID = zdo.GetLong(ZDOVars.s_playerID);
        weaponLoaded = zdo.GetBool(ZDOVars.s_weaponLoaded);
        inDebugFly = zdo.GetBool(ZDOVars.s_debugFly);
        eitr = zdo.GetFloat(ZDOVars.s_eitr);
        stamina = zdo.GetFloat(ZDOVars.s_stamina);
        pvp = zdo.GetBool(ZDOVars.s_pvp);
        inBed = zdo.GetBool(ZDOVars.s_inBed);

        ZNet.PlayerInfo playerInfo = ZNet.instance.GetPlayerList().Find(x => x.m_name.Equals(name));
        var user = PrivilegeManager.ParseUser(playerInfo.m_host);
        platformId = user.id;
        platform = user.platform.ToString();
        publicPosition = playerInfo.m_publicPosition;
        isAdmin = ZNet.instance.PlayerIsAdmin(user.id.ToString());

        var hasWorldObjects = MobulesInstalled.Contains("WorldObjectsData");
        if (hasWorldObjects)
        {
            var inventoryJson = await AskClientForInventory();
            inventory = JSON.ToObject<InventoryData>(inventoryJson);
        }

        return this;
    }

    private async Task<string> AskClientForInventory()
    {
        ZRoutedRpc.instance.InvokeRoutedRPC("VWL_GetPlayerInventoryFromClient", name);
        await Task.Delay(100);

        var maxTimeWait = 2000;
        var alreadyWaited = 0;
        while (true)
        {
            if (alreadyWaited >= maxTimeWait) continue;
            await Task.Delay(100);
            if (!RegisterRPCs.dataJson.TryGetValue(name!, out var data))
            {
                alreadyWaited += 100;
                continue;
            }

            RegisterRPCs.dataJson.Remove(name!);
            return data;
        }
    }
}

[HarmonyWrapSafe, HarmonyPatch]
static class RegisterRPCs
{
    public static readonly Dictionary<string, string> dataJson = [];

    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
    [HarmonyPostfix]
    private static void AddRPCsPatch()
    {
        Debug($"Registering RPCs for {ModName}", ConsoleColor.Yellow);
        var hasWorldObjects = MobulesInstalled.Contains("WorldObjectsData");
        if (!hasWorldObjects) return;

        var zrp = ZRoutedRpc.instance;
        if (!zrp.m_functions.ContainsKey("VWL_GotInventory_Server".GetStableHashCode()))
        {
            Debug($"Registering RPCs for {ModName} - VWL_GotInventory_Server", ConsoleColor.Yellow);
            zrp.Register<string, string>("VWL_GotInventory_Server", OnGetInventory);
        }
    }

    private static void OnGetInventory(long _, string name, string dataJson)
    {
        Debug($"Client sent inventory of {name} as '{dataJson}'");
        RegisterRPCs.dataJson.Add(name, dataJson);
    }
}