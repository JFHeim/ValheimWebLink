using fastJSON;
using HarmonyLib;

namespace VWL_WorldObjectsData.Patch;

[HarmonyWrapSafe, HarmonyPatch]
file static class RegisterRPCs
{
    private static Transform spawnHolder;

    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
    [HarmonyPostfix]
    private static void AddRPCsPatch()
    {
        spawnHolder = new GameObject("VWL_SpawnHolder").transform;
        spawnHolder.gameObject.SetActive(false);

        var zrp = ZRoutedRpc.instance;
        if (!zrp.m_functions.ContainsKey("VWL_SetPlayerData".GetStableHashCode()))
            zrp.Register<string, string>("VWL_SetPlayerData", SetPlayerData);
        if (!zrp.m_functions.ContainsKey("VWL_GetPlayerInventoryFromClient".GetStableHashCode()))
            zrp.Register<string>("VWL_GetPlayerInventoryFromClient", SendInventoryToServer);
    }

    private static void SendInventoryToServer(long _, string name)
    {
        ZRoutedRpc.instance.InvokeRoutedRPC("VWL_GotInventory_Server", name, "test");
    }

    private static void SetPlayerData(long _, string name, string dataJson)
    {
        PlayerData data;
        try
        {
            data = JSON.ToObject<PlayerData>(dataJson);
        }
        catch (Exception e)
        {
            DebugError($"SetPlayerData: Failed to parse PlayerData from JSON: {e}");
            return;
        }

        var pl = m_localPlayer;
        if (!pl || name != pl.GetPlayerName()) return;
        Debug($"Got SetPlayerData for self\n{data.GetObjectString()}");

        pl.m_debugFly = data.inDebugFly;
        pl.SetMaxHealth(data.maxHealth);
        pl.SetHealth(data.health);
        pl.m_eitr = data.eitr;
        pl.m_maxEitr = Max(pl.m_eitr, data.eitr);
        pl.m_stamina = data.stamina;
        pl.m_maxStamina = Max(pl.m_stamina, data.stamina);
        pl.SetPVP(data.pvp);
        pl.SetSleeping(data.inBed);
        pl.m_noiseRange = data.noise;
        pl.m_nview.GetZDO().Set(ZDOVars.s_noise, data.noise);
        pl.TeleportTo(data.position, Quaternion.identity, false);
        ZNet.instance.SetPublicReferencePosition(data.publicPosition);
        ChangeInventory(pl, data.inventory);
    }

    private static void ChangeInventory(Player pl, InventoryData inventoryData)
    {
        var plInv = pl.GetInventory().m_inventory;
        foreach (var item in inventoryData.items)
        {
            var itemData = plInv.Find(x => x.m_gridPos.ToVector2() == item.gridPos);
            if (itemData == null)
            {
                var itemDrop = ObjectDB.instance.GetItem(item.prefabName);
                if (!itemDrop) continue;
                itemDrop = Instantiate(itemDrop, parent: spawnHolder);
                itemDrop.m_itemData.m_gridPos = item.gridPos;
                itemDrop.m_itemData.m_stack = item.stack;
                itemDrop.m_itemData.m_durability = item.durability;
                itemDrop.m_itemData.m_quality = item.quality;
                itemDrop.m_itemData.m_variant = item.variant;
                itemDrop.m_itemData.m_crafterName = item.crafterName;
                plInv.Add(itemDrop.m_itemData);
                Destroy(itemDrop.gameObject);
                continue;
            }

            itemData.m_stack = item.stack;
            itemData.m_durability = item.durability;
            itemData.m_quality = item.quality;
            itemData.m_variant = item.variant;
            itemData.m_crafterName = item.crafterName;
        }
    }


    [Serializable]
    private struct PlayerData
    {
        public SimpleVector3 position;
        public bool publicPosition;
        public bool inDebugFly;
        public float eitr;
        public float stamina;
        public bool pvp;
        public bool inBed;
        public float health;
        public float maxHealth;
        public float noise;
        public InventoryData inventory;
    }
}

[Serializable]
public struct InventoryData()
{
    public List<ItemInfo> items = [];
}

[Serializable]
public struct ItemInfo
{
    public string prefabName;
    public int stack;
    public float durability;
    public int quality;
    public int variant;
    public string crafterName;
    public SimpleVector2 gridPos;
}